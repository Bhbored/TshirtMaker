using TshirtMaker.Models;

namespace TshirtMaker.Services;

public class AuthService
{
    private readonly TestDataService _dataService;
    private User? currentUser;

    public AuthService(TestDataService dataService)
    {
        _dataService = dataService;
    }

    public event Action? OnUserChanged;

    public User? CurrentUser
    {
        get => currentUser;
        private set
        {
            currentUser = value;
            OnUserChanged?.Invoke();
        }
    }

    public bool IsAuthenticated => CurrentUser != null;

    public async Task<User?> Login(string email, string password)
    {
        await Task.Delay(100);

        var user = _dataService.GetAllUsers().FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (user is null)
            return null;

        _dataService.SetCurrentUser(user);
        CurrentUser = user;
        return user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public async Task<User?> SignUp(string email, string username)
    {
        await Task.Delay(100);

        var existingUser = _dataService.GetAllUsers().FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (existingUser != null)
            return null;

        var newUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = username,
            Email = email,
            AvatarUrl = $"https://api.dicebear.com/7.x/avataaars/svg?seed={username}"
        };

        _dataService.AddUser(newUser);
        _dataService.SetCurrentUser(newUser);
        CurrentUser = newUser;
        return newUser;
    }
}
