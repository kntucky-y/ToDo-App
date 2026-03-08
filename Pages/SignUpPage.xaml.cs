using ToDoApp.Models;
using ToDoApp.Services;

namespace ToDoApp.Pages;

public partial class SignUpPage : ContentPage
{
    public SignUpPage()
    {
        InitializeComponent();
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        string userName = UserNameEntry.Text?.Trim() ?? string.Empty;
        string email = EmailEntry.Text?.Trim() ?? string.Empty;
        string password = PasswordEntry.Text ?? string.Empty;
        string confirm = ConfirmEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Required", "Please fill in all fields.", "OK");
            return;
        }

        if (password.Length < 6)
        {
            await DisplayAlert("Weak Password", "Password must be at least 6 characters.", "OK");
            return;
        }

        if (password \!= confirm)
        {
            await DisplayAlert("Mismatch", "Passwords do not match.", "OK");
            return;
        }

        if (AppData.Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            await DisplayAlert("Email Taken", "An account with that email already exists.", "OK");
            return;
        }

        var newUser = new UserModel
        {
            UserId = AppData.NextUserId(),
            UserName = userName,
            Email = email,
            Password = password
        };
        AppData.Users.Add(newUser);

        await DisplayAlert("Account Created", "Your account is ready. Please sign in.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnBackToSignInClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
