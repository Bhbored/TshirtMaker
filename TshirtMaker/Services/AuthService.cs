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

        try
        {
            var user = _dataService.GetAllUsers().FirstOrDefault(x =>
                x.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                x.Password == password);

            if (user is not null)
            {
                CurrentUser = user;
            }

            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login failed: {ex.Message}");
            return null;
        }
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public async Task<User?> SignUp(string email, string username, string password)
    {
        await Task.Delay(100);

        var existingUser = _dataService.GetAllUsers().FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (existingUser != null)
            return null;

        var newUser = new User
        {
            Username = username,
            Password = password,
            Email = email,
            AvatarUrl = $"https://api.dicebear.com/7.x/avataaars/svg?seed={username}"
        };
        CurrentUser = newUser;
        return newUser;
    }
}
