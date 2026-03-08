using ToDoApp.Models;
using ToDoApp.Services;

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

        var user = AppData.Users.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.Password == password);

        if (user is null)
        {
            await DisplayAlert("Sign In Failed", "Email or password is incorrect.", "OK");
            return;
        }

        AppData.CurrentUser = user;
        Application.Current!.Windows[0].Page = new AppShell();
    }

    private async void OnGoToSignUpClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignUpPage());
    }
}
