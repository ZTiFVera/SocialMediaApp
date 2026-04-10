using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SocialMediaApp.Models;
using SocialMediaApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApp.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string  email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;
            Title = "Register";
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var user = new User
                {
                    Username = Username,
                    Email = Email,
                    Password = Password
                };

                var success = await _authService.RegisterAsync(user);
                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Account created!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Registration failed", "OK");
                }
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
