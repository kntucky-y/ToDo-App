using ToDoApp.Services;
using ToDoApp.ViewModels;

namespace ToDoApp.Pages;

public partial class AddTodoPage : ContentPage
{
    public AddTodoPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        TitleEntry.Text = string.Empty;
        DetailsEditor.Text = string.Empty;
        Dispatcher.Dispatch(() => TitleEntry.Focus());
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Required", "Please enter a task title.", "OK");
            TitleEntry.Focus();
            return;
        }

        int userId = AppData.CurrentUser?.UserId ?? 0;
        if (userId <= 0)
        {
            await DisplayAlert("Session Expired", "Please sign in again.", "OK");
            return;
        }

        var (ok, message, item) = await ToDoApiService.AddItemAsync(
            title,
            DetailsEditor.Text?.Trim() ?? string.Empty,
            userId);

        if (!ok || item is null)
        {
            await DisplayAlert("Add Failed", message, "OK");
            return;
        }

        AppData.PendingItems.Add(item);
        AppViewModel.Instance.RefreshComputedFlags();
        await Shell.Current.GoToAsync("..");
    }
}
