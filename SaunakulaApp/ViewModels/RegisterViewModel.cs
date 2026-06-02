using CommunityToolkit.Mvvm.Input;
using SaunakulaApp.Models;
using SaunakulaApp.Services;

namespace SaunakulaApp.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly SupabaseService _supabaseService;
    private readonly SessionService _sessionService;

    private string _fullName = string.Empty;
    private string _email = string.Empty;
    private string _phone = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _hasError;

    public IAsyncRelayCommand InitializeCommand { get; }
    public IAsyncRelayCommand RegisterCommand { get; }
    public IAsyncRelayCommand GoToLoginCommand { get; }

    public RegisterViewModel(DatabaseService databaseService, SupabaseService supabaseService, SessionService sessionService)
    {
        _databaseService = databaseService;
        _supabaseService = supabaseService;
        _sessionService = sessionService;

        InitializeCommand = new AsyncRelayCommand(InitializeAsync);
        RegisterCommand = new AsyncRelayCommand(RegisterAsync);
        GoToLoginCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(".."));
    }

    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
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

    private async Task RegisterAsync()
    {
        var name = FullName.Trim();
        var email = Email.Trim();
        var phone = Phone.Trim();
        var password = Password;
        var confirm = ConfirmPassword;

        if (string.IsNullOrEmpty(name) ||
            string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password))
        {
            ShowError("Palun täida kõik kohustuslikud väljad.");
            return;
        }

        if (password != confirm)
        {
            ShowError("Paroolid ei ühti.");
            return;
        }

        if (password.Length < 6)
        {
            ShowError("Parool peab olema vähemalt 6 tähemärki.");
            return;
        }

        try
        {
            IsBusy = true;

            var existingRemote = await _supabaseService.GetAppUserByEmailAsync(email);
            if (existingRemote is not null)
            {
                ShowError("See e-posti aadress on juba kasutusel.");
                return;
            }

            var user = new User
            {
                FullName = name,
                Email = email,
                Phone = phone,
                PasswordHash = PasswordService.HashPassword(password),
                CreatedAt = DateTime.Now
            };

            var createdRemote = await _supabaseService.RegisterAppUserAsync(user);

            var created = new User
            {
                Id = checked((int)createdRemote.Id),
                FullName = createdRemote.FullName,
                Email = createdRemote.Email,
                Phone = createdRemote.Phone,
                PasswordHash = createdRemote.PasswordHash,
                IsVip = createdRemote.IsVip,
                VipGrantedAt = createdRemote.VipGrantedAt,
                CreatedAt = createdRemote.CreatedAt == default ? DateTime.Now : createdRemote.CreatedAt
            };

            _sessionService.Login(created);

            ClearSensitiveFields();
            await Shell.Current.GoToAsync("//HomePage");
        }
        catch (Exception ex)
        {
            ShowError($"Registration failed: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ClearSensitiveFields()
    {
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        ErrorMessage = string.Empty;
        HasError = false;
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }
}
