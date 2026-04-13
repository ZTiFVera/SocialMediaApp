using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SocialMediaApp.Models;
using static Java.Interop.JniEnvironment;

namespace SocialMediaApp.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string bio = string.Empty;

        [ObservableProperty]
        private int postsCount;

        [ObservableProperty]
        private bool isEditing;

        public ProfileViewModel()
        {
            Title = "Profile";
            LoadProfile();
        }

        private void LoadProfile()
        {
            Username = Preferences.Get("username", "User");
            Email = Preferences.Get("email", "user@email.com");
            Bio = Preferences.Get("bio", "Hey there! I'm using SocialApp.");
            PostsCount = Preferences.Get("postsCount", 0);
        }

        [RelayCommand]
        private void EditProfile()
        {
            IsEditing = true;
        }

        [RelayCommand]
        private async Task SaveProfileAsync()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                await Shell.Current.DisplayAlert("Error", "Username cannot be empty", "OK");
                return;
            }

            Preferences.Set("username", Username);
            Preferences.Set("email", Email);
            Preferences.Set("bio", Bio);

            IsEditing = false;
            await Shell.Current.DisplayAlert("Success", "Profile updated!", "OK");
        }

        [RelayCommand]
        private void CancelEdit()
        {
            LoadProfile();
            IsEditing = false;
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Logout", "Are you sure you want to logout?", "Yes", "No");
            if (confirm)
                await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}