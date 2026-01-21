using TshirtMaker.Models.Core;
using TshirtMaker.Services.Supabase;

namespace TshirtMaker.Services;

public class AuthStateService
{
    private readonly SupabaseAuthService _supabaseAuth;
    private User? _currentUser = null;

    public AuthStateService(SupabaseAuthService supabaseAuth)
    {
        _supabaseAuth = supabaseAuth;
    }

    public event Action? OnUserChanged;

    public User? CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            OnUserChanged?.Invoke();
        }
    }

    public bool IsAuthenticated => CurrentUser != null;

    public async Task<User?> InitializeAsync()
    {
        if (CurrentUser != null)
        {
            return CurrentUser;
        }

        var userDto = await _supabaseAuth.GetCurrentUserAsync();
        if (userDto != null)
        {
            CurrentUser = userDto.ToModel();
            return CurrentUser;
        }
        CurrentUser = null;
        return null;
    }

    public void SetUser(User user)
    {
        CurrentUser = user;
    }

    public void ClearUser()
    {
        CurrentUser = null;
    }
}
