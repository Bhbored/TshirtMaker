using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using TshirtMaker.DTOs;

namespace TshirtMaker.Services.Supabase
{
    public class SupabaseAuthService
    {
        private readonly global::Supabase.Client _supabase;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "supabase_access_token";
        private const string RefreshTokenKey = "supabase_refresh_token";

        public SupabaseAuthService(global::Supabase.Client supabase, IHttpContextAccessor httpContextAccessor)
        {
            _supabase = supabase;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserDto?> SignUpAsync(string email, string password, Dictionary<string, object>? metadata = null)
        {
            var options = new SignUpOptions
            {
                Data = metadata ?? new Dictionary<string, object>()
            };

            var response = await _supabase.Auth.SignUp(email, password, options);
            if (response?.User == null) return null;

            StoreSession(response.AccessToken, response.RefreshToken);
            return MapToUserDto(response.User);
        }

        public async Task<UserDto?> SignInAsync(string email, string password)
        {
            var response = await _supabase.Auth.SignIn(email, password);
            if (response?.User == null) return null;

            StoreSession(response.AccessToken, response.RefreshToken);
            return MapToUserDto(response.User);
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            var accessToken = GetStoredAccessToken();
            if (string.IsNullOrEmpty(accessToken)) return null;

            try
            {
                var user = await _supabase.Auth.GetUser(accessToken);
                return user != null ? MapToUserDto(user) : null;
            }
            catch
            {
                await TryRefreshSessionAsync();
                var newAccessToken = GetStoredAccessToken();
                if (string.IsNullOrEmpty(newAccessToken)) return null;

                try
                {
                    var user = await _supabase.Auth.GetUser(newAccessToken);
                    return user != null ? MapToUserDto(user) : null;
                }
                catch
                {
                    ClearSession();
                    return null;
                }
            }
        }

        public Session? GetSession()
        {
            var accessToken = GetStoredAccessToken();
            var refreshToken = GetStoredRefreshToken();

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                return null;

            return _supabase.Auth.CurrentSession;
        }

        public async Task<bool> TryRefreshSessionAsync()
        {
            var refreshToken = GetStoredRefreshToken();
            if (string.IsNullOrEmpty(refreshToken)) return false;

            try
            {
                var response = await _supabase.Auth.RefreshSession();
                if (response?.AccessToken != null)
                {
                    StoreSession(response.AccessToken, response.RefreshToken);
                    return true;
                }
            }
            catch
            {
                ClearSession();
            }

            return false;
        }

        public async Task SignOutAsync()
        {
            try
            {
                await _supabase.Auth.SignOut();
            }
            catch { }

            ClearSession();
        }

        public async Task<UserDto?> UpdateUserAsync(Dictionary<string, object> attributes)
        {
            var accessToken = GetStoredAccessToken();
            if (string.IsNullOrEmpty(accessToken)) return null;

            var userAttributes = new UserAttributes
            {
                Data = attributes
            };

            var user = await _supabase.Auth.Update(userAttributes);
            return user != null ? MapToUserDto(user) : null;
        }

        public async Task<bool> ResetPasswordForEmailAsync(string email)
        {
            try
            {
                await _supabase.Auth.ResetPasswordForEmail(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void StoreSession(string? accessToken, string? refreshToken)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            if (!string.IsNullOrEmpty(accessToken))
                session.SetString(SessionKey, accessToken);

            if (!string.IsNullOrEmpty(refreshToken))
                session.SetString(RefreshTokenKey, refreshToken);
        }

        private string? GetStoredAccessToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString(SessionKey);
        }

        private string? GetStoredRefreshToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString(RefreshTokenKey);
        }

        private void ClearSession()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            session.Remove(SessionKey);
            session.Remove(RefreshTokenKey);
        }

        private UserDto MapToUserDto(User user)
        {
            var metadata = user.UserMetadata ?? new Dictionary<string, object>();

            var userModel = new TshirtMaker.Models.Core.User
            {
                Id = Guid.Parse(user.Id),
                Email = user.Email ?? string.Empty,
                Username = metadata.TryGetValue("username", out var username)
                    ? username?.ToString() ?? string.Empty
                    : user.Email?.Split('@')[0] ?? string.Empty,
                AvatarUrl = metadata.TryGetValue("avatar_url", out var avatarUrl)
                    ? avatarUrl?.ToString() ?? string.Empty
                    : string.Empty,
                Bio = metadata.TryGetValue("bio", out var bio)
                    ? bio?.ToString()
                    : null,
                Location = metadata.TryGetValue("location", out var location)
                    ? location?.ToString()
                    : null,
                WebsiteUrl = metadata.TryGetValue("website_url", out var websiteUrl)
                    ? websiteUrl?.ToString()
                    : null,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt ?? DateTime.UtcNow,
                LastLoginAt = user.LastSignInAt ?? DateTime.UtcNow
            };

            return UserDto.FromModel(userModel);
        }
    }
}


