//using TshirtMaker.Models;

//namespace TshirtMaker.Services.Supabase;

//public class SupabaseAuthService
//{
//    private readonly string _supabaseUrl;
//    private readonly string _supabaseKey;

//    public SupabaseAuthService(IConfiguration configuration)
//    {
//        _supabaseUrl = configuration["Supabase:Url"] ?? string.Empty;
//        _supabaseKey = configuration["Supabase:Key"] ?? string.Empty;
//    }

//    public async Task<User?> SignUp(string email, string password, string username)
//    {
//        await Task.Delay(100);
//        return null;
//    }

//    public async Task<User?> SignIn(string email, string password)
//    {
//        await Task.Delay(100);
//        return null;
//    }

//    public async Task SignOut()
//    {
//        await Task.Delay(100);
//    }

//    public async Task<User?> GetCurrentUser()
//    {
//        await Task.Delay(100);
//        return null;
//    }
//}
