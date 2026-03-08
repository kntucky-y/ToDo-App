using ToDoApp.ViewModels;

namespace ToDoApp.Pages;

public partial class CompletedPage : ContentPage
{
    public CompletedPage()
    {
        InitializeComponent();
        BindingContext = AppViewModel.Instance;
    }
}
