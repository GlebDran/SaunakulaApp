using CommunityToolkit.Mvvm.Input;
using SaunakulaApp.Services;
using SaunakulaApp.Views;

namespace SaunakulaApp.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly SessionService _sessionService;

    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _hasError;

    public IAsyncRelayCommand InitializeCommand { get; }
    public IAsyncRelayCommand LoginCommand { get; }
    public IAsyncRelayCommand GoToRegisterCommand { get; }

    public LoginViewModel(DatabaseService databaseService, SessionService sessionService)
    {
        _databaseService = databaseService;
        _sessionService = sessionService;

        InitializeCommand = new AsyncRelayCommand(InitializeAsync);
        LoginCommand = new AsyncRelayCommand(LoginAsync);
        GoToRegisterCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(nameof(RegisterPage)));
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public bool HasError
    {
        get => _hasError;
        private set => SetProperty(ref _hasError, value);
    }

    public Task InitializeAsync()
        => _databaseService.InitAsync();

    private async Task LoginAsync()
    {
        var email = Email.Trim();
        var password = Password;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Palun t\u00e4ida k\u00f5ik v\u00e4ljad.");
            return;
        }

        try
        {
            IsBusy = true;

            var user = await _databaseService.GetUserByEmailAsync(email);
            if (user is null || user.PasswordHash != PasswordService.HashPassword(password))
            {
                ShowError("Vale e-post v\u00f5i parool.");
                return;
            }

            _sessionService.Login(user);
            ErrorMessage = string.Empty;
            HasError = false;
            Password = string.Empty;

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }
}
