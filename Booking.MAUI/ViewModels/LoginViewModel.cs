using Booking.App.DTOs;
using Booking.MAUI.Service;
using System.Windows.Input;

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
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotLoggedIn)); // Uppdatera motsvarande egenskap
        }
    }

    public bool IsNotLoggedIn => !IsLoggedIn;

    public ICommand LoginCommand { get; }
    public ICommand LogoutCommand { get; }

    private async Task LoginAsync()
    {
        try
        {
            var loginDto = new LoginDto { UserName = UserName, Password = Password };
            var token = await _authService.LoginAsync(loginDto);

            await SecureStorage.SetAsync("auth_token", token);
            IsLoggedIn = true;

            await Shell.Current.GoToAsync("//HomePage");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task LogoutAsync()
    {
        try
        {
            await _authService.LogoutUserAsync();
            SecureStorage.Remove("auth_token");
            IsLoggedIn = false;

            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
