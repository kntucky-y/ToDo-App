using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using ToDoApp.Models;

namespace ToDoApp.Services;

public static class ToDoApiService
{
    private static readonly HttpClient Http = new()
    {
        BaseAddress = new Uri("https://todo-list.dcism.org/")
    };

    public static async Task<(bool Ok, string Message, UserModel? User)> SignInAsync(string email, string password)
    {
        string url = $"signin_action.php?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}";
        var response = await Http.GetAsync(url);
        var root = await ReadJsonAsync(response);

        int status = ReadStatusCode(root, (int)response.StatusCode);
        string message = ReadMessage(root, "Unable to sign in.");
        if (status != 200)
        {
            return (false, message, null);
        }

        var data = root?["data"]?.AsObject();
        if (data is null)
        {
            return (false, "Malformed sign in response.", null);
        }

        string firstName = data["fname"]?.GetValue<string>() ?? string.Empty;
        string lastName = data["lname"]?.GetValue<string>() ?? string.Empty;
        var user = new UserModel
        {
            UserId = data["id"]?.GetValue<int>() ?? 0,
            UserName = string.Join(" ", new[] { firstName, lastName }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim(),
            Email = data["email"]?.GetValue<string>() ?? email,
            Password = password
        };

        return (true, message, user);
    }

    public static async Task<(bool Ok, string Message)> SignUpAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        string confirmPassword)
    {
        var payload = new
        {
            first_name = firstName,
            last_name = lastName,
            email,
            password,
            confirm_password = confirmPassword
        };

        var response = await Http.PostAsJsonAsync("signup_action.php", payload);
        var root = await ReadJsonAsync(response);
        int status = ReadStatusCode(root, (int)response.StatusCode);
        string message = ReadMessage(root, "Unable to sign up.");
        return (status == 200, message);
    }

    public static async Task<(bool Ok, string Message, List<ToDoClass> Items)> GetItemsAsync(string status, int userId)
    {
        string url = $"getItems_action.php?status={Uri.EscapeDataString(status)}&user_id={userId}";
        var response = await Http.GetAsync(url);
        var root = await ReadJsonAsync(response);

        int code = ReadStatusCode(root, (int)response.StatusCode);
        string message = ReadMessage(root, string.Empty);
        if (code != 200)
        {
            return (false, string.IsNullOrWhiteSpace(message) ? "Unable to load tasks." : message, new List<ToDoClass>());
        }

        var items = new List<ToDoClass>();
        var data = root?["data"];

        if (data is JsonObject obj)
        {
            foreach (var kv in obj)
            {
                if (kv.Value is JsonObject itemObj)
                {
                    items.Add(ParseItem(itemObj));
                }
            }
        }
        else if (data is JsonArray arr)
        {
            foreach (var node in arr)
            {
                if (node is JsonObject itemObj)
                {
                    items.Add(ParseItem(itemObj));
                }
            }
        }

        return (true, message, items);
    }

    public static async Task<(bool Ok, string Message, ToDoClass? Item)> AddItemAsync(string itemName, string itemDescription, int userId)
    {
        var payload = new
        {
            item_name = itemName,
            item_description = itemDescription,
            user_id = userId
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "addItem_action.php")
        {
            Content = JsonContent.Create(payload)
        };

        var response = await Http.SendAsync(request);
        var root = await ReadJsonAsync(response);
        int status = ReadStatusCode(root, (int)response.StatusCode);
        string message = ReadMessage(root, "Unable to add task.");
        if (status != 200)
        {
            return (false, message, null);
        }

        var data = root?["data"]?.AsObject();
        return (true, message, data is null ? null : ParseItem(data));
    }

    public static async Task<(bool Ok, string Message)> UpdateItemAsync(int itemId, string itemName, string itemDescription)
    {
        var payload = new
        {
            item_name = itemName,
            item_description = itemDescription,
            item_id = itemId
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "editItem_action.php")
        {
            Content = JsonContent.Create(payload)
        };

        var response = await Http.SendAsync(request);
        var root = await ReadJsonAsync(response);
        int status = ReadStatusCode(root, (int)response.StatusCode);
        string message = ReadMessage(root, "Unable to update task.");
        return (status == 200, message);
    }

    public static async Task<(bool Ok, string Message)> ChangeStatusAsync(int itemId, string status)
    {
        var payload = new
        {
            status,
            item_id = itemId
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "statusItem_action.php")
        {
            Content = JsonContent.Create(payload)
        };

        var response = await Http.SendAsync(request);
        var root = await ReadJsonAsync(response);
        int code = ReadStatusCode(root, (int)response.StatusCode);
        string message = ReadMessage(root, "Unable to change task status.");
        return (code == 200, message);
    }

    public static async Task<(bool Ok, string Message)> DeleteItemAsync(int itemId)
    {
        string url = $"deleteItem_action.php?item_id={itemId}";
        var response = await Http.DeleteAsync(url);
        var root = await ReadJsonAsync(response);
        int status = ReadStatusCode(root, (int)response.StatusCode);
        string message = ReadMessage(root, "Unable to delete task.");

        if (status == 200)
        {
            return (true, message);
        }

        // Some PHP backends reject DELETE verbs; fall back to GET shape in the document.
        var fallback = await Http.GetAsync(url);
        var fallbackRoot = await ReadJsonAsync(fallback);
        int fallbackStatus = ReadStatusCode(fallbackRoot, (int)fallback.StatusCode);
        string fallbackMessage = ReadMessage(fallbackRoot, message);
        return (fallbackStatus == 200, fallbackMessage);
    }

    private static int ReadStatusCode(JsonObject? root, int fallback)
    {
        if (root is null) return fallback;
        return ReadInt(root, "status", fallback);
    }

    private static string ReadMessage(JsonObject? root, string fallback)
    {
        if (root is null) return fallback;
        string message = ReadString(root, "message");
        return string.IsNullOrWhiteSpace(message) ? fallback : message;
    }

    private static ToDoClass ParseItem(JsonObject obj)
    {
        return new ToDoClass
        {
            item_id = ReadInt(obj, "item_id"),
            item_name = ReadString(obj, "item_name"),
            item_description = ReadString(obj, "item_description"),
            status = ReadString(obj, "status"),
            user_id = ReadInt(obj, "user_id")
        };
    }

    private static int ReadInt(JsonObject obj, string key, int fallback = 0)
    {
        var node = obj[key];
        if (node is null) return fallback;
        if (node is JsonValue intValue && intValue.TryGetValue<int>(out var n)) return n;
        if (node is JsonValue stringValue && stringValue.TryGetValue<string>(out var s) && int.TryParse(s, out var p)) return p;
        return fallback;
    }

    private static string ReadString(JsonObject obj, string key)
    {
        var node = obj[key];
        if (node is null) return string.Empty;
        if (node is JsonValue value && value.TryGetValue<string>(out var s)) return s;
        return node.ToJsonString();
    }

    private static async Task<JsonObject?> ReadJsonAsync(HttpResponseMessage response)
    {
        try
        {
            var root = await response.Content.ReadFromJsonAsync<JsonObject>();
            return root;
        }
        catch
        {
            return new JsonObject
            {
                ["status"] = (int)response.StatusCode,
                ["message"] = response.StatusCode == HttpStatusCode.OK
                    ? "Success"
                    : "Request failed."
            };
        }
    }
}