using TshirtMaker.Models;

namespace TshirtMaker.Services;

public class AuthService
{
    private readonly TestDataService _dataService;
    private User? currentUser = null;
    private User? defaultUser = null;

    public AuthService(TestDataService dataService)
    {
        _dataService = dataService;
        var allUsers = _dataService.GetAllUsers();
        defaultUser = allUsers.FirstOrDefault();
    }

    public event Action? OnUserChanged;

    public User? CurrentUser
    {
        get => currentUser;
        set
        {
            currentUser = value;
            OnUserChanged?.Invoke();
        }
    }

    public bool IsAuthenticated => CurrentUser != null && defaultUser != null && CurrentUser.Id != defaultUser.Id;

    public async Task<User?> Login(string email, string password)
    {
        await Task.Delay(100);
        var user = _dataService.GetAllUsers().FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (user != null)
        {
            _dataService.SetCurrentUser(user);
            CurrentUser = user;
            return user;
        }

        return null;
    }

    public void Logout()
    {
        if (CurrentUser != null && defaultUser != null)
        {
            _dataService.SetCurrentUser(defaultUser);
            CurrentUser = null;
        }
    }

    public void Initialize()
    {
        var user = _dataService.GetCurrentUser();
        if (user != null && defaultUser != null && user.Id != defaultUser.Id)
        {
            CurrentUser = user;
        }
        else
        {
            CurrentUser = null;
        }
    }
}
