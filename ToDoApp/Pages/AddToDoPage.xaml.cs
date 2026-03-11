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

        var item = new ToDoClass
        {
            item_id = AppData.NextItemId(),
            item_name = title,
            item_description = DetailsEditor.Text?.Trim() ?? string.Empty,
            status = "pending",
            user_id = AppData.CurrentUser?.UserId ?? 0
        };

        AppData.PendingItems.Add(item);
        await Shell.Current.GoToAsync("..");
    }
}
