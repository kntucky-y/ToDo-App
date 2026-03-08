using ToDoApp.Services;
using ToDoApp.ViewModels;

namespace ToDoApp.Pages;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var user = AppData.CurrentUser;
        if (user != null)
        {
            UserNameLabel.Text = user.UserName;
            EmailLabel.Text = user.Email;
            AvatarLabel.Text = user.UserName.Length > 0
                ? user.UserName[0].ToString().ToUpper()
                : "U";
        }
        PendingCountLabel.Text = AppViewModel.Instance.PendingItems.Count.ToString();
        CompletedCountLabel.Text = AppViewModel.Instance.CompletedItems.Count.ToString();
    }

    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        bool confirmed = await DisplayAlert("Sign Out", "Are you sure you want to sign out?", "Sign Out", "Cancel");
        if (!confirmed) return;

        AppData.CurrentUser = null;
        AppData.PendingItems.Clear();
        AppData.CompletedItems.Clear();
        Application.Current!.Windows[0].Page = new NavigationPage(new SignInPage())
        {
            BarBackgroundColor = Color.FromArgb("#7B5EA7"),
            BarTextColor = Colors.White
        };
    }
}
