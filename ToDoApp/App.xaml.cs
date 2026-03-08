using ToDoApp.Pages;

namespace ToDoApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
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
