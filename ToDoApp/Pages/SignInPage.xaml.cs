using ToDoApp.Models;
using ToDoApp.Services;
using ToDoApp.ViewModels;

namespace ToDoApp.Pages;

public partial class SignInPage : ContentPage
{
    public SignInPage()
    {
        InitializeComponent();
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? string.Empty;
        string password = PasswordEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Required", "Please enter your email and password.", "OK");
            return;
        }

        var (ok, message, user) = await ToDoApiService.SignInAsync(email, password);
        if (!ok || user is null)
        {
            await DisplayAlert("Sign In Failed", message, "OK");
            return;
        }

        AppData.CurrentUser = user;

        var active = await ToDoApiService.GetItemsAsync("active", user.UserId);
        var inactive = await ToDoApiService.GetItemsAsync("inactive", user.UserId);

        AppData.PendingItems.Clear();
        AppData.CompletedItems.Clear();

        if (active.Ok)
        {
            foreach (var item in active.Items)
            {
                AppData.PendingItems.Add(item);
            }
        }

        if (inactive.Ok)
        {
            foreach (var item in inactive.Items)
            {
                AppData.CompletedItems.Add(item);
            }
        }

        AppViewModel.Instance.RefreshComputedFlags();
        Application.Current!.Windows[0].Page = new AppShell();
    }

    private async void OnGoToSignUpClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignUpPage());
    }
}
