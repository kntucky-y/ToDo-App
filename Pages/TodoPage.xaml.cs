using ToDoApp.ViewModels;

namespace ToDoApp.Pages;

public partial class TodoPage : ContentPage
{
    public TodoPage()
    {
        InitializeComponent();
        BindingContext = AppViewModel.Instance;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        TodoListView.SelectedItem = null;
    }
}
