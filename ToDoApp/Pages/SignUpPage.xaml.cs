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
        string firstName = FirstNameEntry.Text?.Trim() ?? string.Empty;
        string lastName = LastNameEntry.Text?.Trim() ?? string.Empty;
        string email = EmailEntry.Text?.Trim() ?? string.Empty;
        string password = PasswordEntry.Text ?? string.Empty;
        string confirm = ConfirmEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email) ||
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

        if (password != confirm)
        {
            await DisplayAlert("Mismatch", "Passwords do not match.", "OK");
            return;
        }

        var (ok, message) = await ToDoApiService.SignUpAsync(firstName, lastName, email, password, confirm);
        if (!ok)
        {
            await DisplayAlert("Sign Up Failed", message, "OK");
            return;
        }

        await DisplayAlert("Account Created", string.IsNullOrWhiteSpace(message) ? "Your account is ready. Please sign in." : message, "OK");
        await Navigation.PopAsync();
    }

    private async void OnBackToSignInClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
