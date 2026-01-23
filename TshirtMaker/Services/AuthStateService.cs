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
        // Check if we already have a user in memory
        if (CurrentUser != null)
        {
            return CurrentUser;
        }

        try
        {
            // Try to get the current user from Supabase (which will use session data)
            var userDto = await _supabaseAuth.GetCurrentUserAsync();
            if (userDto != null)
            {
                CurrentUser = userDto.ToModel();
                return CurrentUser;
            }
        }
        catch (Exception ex)
        {
            // Log the exception but don't crash
            Console.WriteLine($"Error initializing auth state: {ex.Message}");
        }

        // If we couldn't get the user, ensure we clear the current user
        CurrentUser = null;
        return null;
    }

    // Method to force refresh the auth state from Supabase
    public async Task<User?> RefreshAsync()
    {
        // Clear current user first
        CurrentUser = null;

        // Then try to initialize again
        return await InitializeAsync();
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
