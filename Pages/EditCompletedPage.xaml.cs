using ToDoApp.Helpers;
using ToDoApp.Services;
using ToDoApp.ViewModels;

namespace ToDoApp.Pages;

public partial class EditCompletedPage : ContentPage
{
    public EditCompletedPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var item = NavigationContext.EditingItem;
        if (item \!= null)
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
        if (item \!= null)
        {
            item.item_name = title;
            item.item_description = DetailsEditor.Text?.Trim() ?? string.Empty;
        }
        NavigationContext.EditingItem = null;
        await Shell.Current.GoToAsync("..");
    }

    private async void OnIncompleteClicked(object sender, EventArgs e)
    {
        var item = NavigationContext.EditingItem;
        if (item \!= null)
        {
            AppViewModel.Instance.UndoCompletedCommand.Execute(item);
        }
        NavigationContext.EditingItem = null;
        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool ok = await DisplayAlert("Delete", "Delete this task?", "Delete", "Cancel");
        if (\!ok) return;
        var item = NavigationContext.EditingItem;
        if (item \!= null)
        {
            AppData.CompletedItems.Remove(item);
        }
        NavigationContext.EditingItem = null;
        await Shell.Current.GoToAsync("..");
    }
}
