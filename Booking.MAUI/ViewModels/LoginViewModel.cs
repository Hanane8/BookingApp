using Booking.App.DTOs;
using Booking.MAUI.Service;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class LoginViewModel : BindableObject
{
    private readonly AuthService _authService;
    private string _userName;
    private string _password;
    private bool _isLoggedIn;

    public LoginViewModel(AuthService authService)
    {
        _authService = authService;
        LoginCommand = new Command(async () => await LoginAsync());
        LogoutCommand = new Command(async () => await LogoutAsync());

       
        IsLoggedIn = !string.IsNullOrEmpty(SecureStorage.GetAsync("auth_token").Result);
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

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set
        {
            _isLoggedIn = value;
            OnPropertyChanged(nameof(IsLoggedIn)); // Skicka med namnet på egenskapen
            OnPropertyChanged(nameof(IsNotLoggedIn)); // Update the opposite property for visibility
        }
    }

    // This is the opposite of IsLoggedIn and helps to control visibility of Login button
    public bool IsNotLoggedIn => !IsLoggedIn;

    public ICommand LoginCommand { get; }
    public ICommand LogoutCommand { get; }

    // Method for logging in the user
    private async Task LoginAsync()
    {
        try
        {
            var loginDto = new LoginDto { UserName = UserName, Password = Password };
            var token = await _authService.LoginAsync(loginDto);

            // Store the token securely
            await SecureStorage.SetAsync("auth_token", token);
            IsLoggedIn = true;  // Set logged-in state to true

            // Navigate to HomePage after login
            await Shell.Current.GoToAsync("//HomePage");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // Method for logging out the user
    private async Task LogoutAsync()
    {
        try
        {
            await _authService.LogoutUserAsync();  // Perform logout via the service
            SecureStorage.Remove("auth_token");  // Remove the token from secure storage
            IsLoggedIn = false;  // Set logged-out state to false

            // Navigate to LoginPage after logout
            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
