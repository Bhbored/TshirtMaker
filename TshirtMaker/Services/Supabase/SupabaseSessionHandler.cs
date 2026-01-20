using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace TshirtMaker.Services.Supabase
{
    public class SupabaseSessionHandler : IGotrueSessionPersistence<Session>
    {
        private Session? _session;

        public void SaveSession(Session session)
        {
            _session = session;
        }

        public void DestroySession()
        {
            _session = null;
        }

        public Session? LoadSession()
        {
            return _session;
        }
    }
}
