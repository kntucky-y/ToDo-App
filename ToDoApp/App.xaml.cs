using ToDoApp.Pages;

namespace ToDoApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Routing.RegisterRoute("addtodo", typeof(AddTodoPage));
        Routing.RegisterRoute("edittodo", typeof(EditTodoPage));
        Routing.RegisterRoute("editcompleted", typeof(EditCompletedPage));
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var navPage = new NavigationPage(new SignInPage())
        {
            BarBackgroundColor = Color.FromArgb("#7B5EA7"),
            BarTextColor = Colors.White
        };
        return new Window(navPage);
    }
}
