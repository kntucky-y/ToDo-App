using ToDoApp.Helpers;
using ToDoApp.Services;
using ToDoApp.ViewModels;

namespace ToDoApp.Pages;

public partial class EditTodoPage : ContentPage
{
    public EditTodoPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var item = NavigationContext.EditingItem;
        if (item != null)
        {
            TitleEntry.Text = item.item_name;
            DetailsEditor.Text = item.item_description;
        }
    }

    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Required", "Please enter a task title.", "OK");
            return;
        }
        var item = NavigationContext.EditingItem;
        if (item != null)
        {
            var (ok, message) = await ToDoApiService.UpdateItemAsync(
                item.item_id,
                title,
                DetailsEditor.Text?.Trim() ?? string.Empty);
            if (!ok)
            {
                await DisplayAlert("Update Failed", message, "OK");
                return;
            }

            item.item_name = title;
            item.item_description = DetailsEditor.Text?.Trim() ?? string.Empty;
        }
        NavigationContext.EditingItem = null;
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCompleteClicked(object sender, EventArgs e)
    {
        var item = NavigationContext.EditingItem;
        if (item != null)
        {
            var (ok, message) = await ToDoApiService.ChangeStatusAsync(item.item_id, "inactive");
            if (!ok)
            {
                await DisplayAlert("Status Update Failed", message, "OK");
                return;
            }

            AppViewModel.Instance.CompleteTodoCommand.Execute(item);
        }
        NavigationContext.EditingItem = null;
        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool ok = await DisplayAlert("Delete", "Delete this task?", "Delete", "Cancel");
        if (!ok) return;
        var item = NavigationContext.EditingItem;
        if (item != null)
        {
            var (deleted, message) = await ToDoApiService.DeleteItemAsync(item.item_id);
            if (!deleted)
            {
                await DisplayAlert("Delete Failed", message, "OK");
                return;
            }

            AppData.PendingItems.Remove(item);
            AppViewModel.Instance.RefreshComputedFlags();
        }
        NavigationContext.EditingItem = null;
        await Shell.Current.GoToAsync("..");
    }
}
