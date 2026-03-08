using ToDoApp.Pages;

namespace ToDoApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new SignInPage())
        {
            BarBackgroundColor = Color.FromArgb("#7B5EA7"),
            BarTextColor = Colors.White
        };
    }
}
