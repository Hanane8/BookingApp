using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Booking.App.DTOs;
using Booking.App.Services;
using Booking.MAUI.Service;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Booking.MAUI.ViewModels
{
    public class LoginViewModel : BindableObject
    {
        private readonly AuthService _authService;
        private string _userName;
        private string _password;

        public LoginViewModel()
        {
            // If AuthService is registered in DI, resolve it here
            _authService = DependencyService.Get<AuthService>();
            LoginCommand = new Command(async () => await LoginAsync());
        }

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(async () => await LoginAsync());
        }
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }



        private async Task LoginAsync()
        {
            try
            {
                // Logga för att se om metoden körs
                Console.WriteLine("LoginAsync metoden anropades!");

                var loginDto = new LoginDto { UserName = UserName, Password = Password };
                var token = await _authService.LoginAsync(loginDto);
                Console.WriteLine("Login successful, token received.");
                // Hantera token (t.ex. lagra det säkert)
                await SecureStorage.SetAsync("auth_token", token);
                await Shell.Current.GoToAsync("//HomePage");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid inloggning: {ex.Message}");
               
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

    }
}