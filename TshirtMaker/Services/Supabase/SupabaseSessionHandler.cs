using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using Microsoft.AspNetCore.Http;

namespace TshirtMaker.Services.Supabase
{
    public class SupabaseSessionHandler : IGotrueSessionPersistence<Session>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "supabase_session";

        public SupabaseSessionHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SaveSession(Session session)
        {
            // Only save session if we have a valid HttpContext and the response hasn't started
            if (_httpContextAccessor.HttpContext != null &&
                !_httpContextAccessor.HttpContext.Response.HasStarted &&
                session != null)
            {
                var sessionJson = System.Text.Json.JsonSerializer.Serialize(session);
                _httpContextAccessor.HttpContext.Session.SetString(SessionKey, sessionJson);
            }
        }

        public void DestroySession()
        {
            if (_httpContextAccessor.HttpContext != null &&
                !_httpContextAccessor.HttpContext.Response.HasStarted)
            {
                _httpContextAccessor.HttpContext.Session.Remove(SessionKey);
            }
        }

        public Session? LoadSession()
        {
            if (_httpContextAccessor.HttpContext?.Session == null)
                return null;

            var sessionJson = _httpContextAccessor.HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(sessionJson))
                return null;

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<Session>(sessionJson);
            }
            catch
            {
                // If deserialization fails, remove the corrupted session data
                if (_httpContextAccessor.HttpContext != null &&
                    !_httpContextAccessor.HttpContext.Response.HasStarted)
                {
                    _httpContextAccessor.HttpContext.Session.Remove(SessionKey);
                }
                return null;
            }
        }
    }
}
