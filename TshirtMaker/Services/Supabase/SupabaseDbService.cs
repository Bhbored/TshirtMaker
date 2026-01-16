//using TshirtMaker.Models;

//namespace TshirtMaker.Services.Supabase;

//public class SupabaseDbService
//{
//    private readonly string _supabaseUrl;
//    private readonly string _supabaseKey;

//    public SupabaseDbService(IConfiguration configuration)
//    {
//        _supabaseUrl = configuration["Supabase:Url"] ?? string.Empty;
//        _supabaseKey = configuration["Supabase:Key"] ?? string.Empty;
//    }

//    public async Task<Design?> CreateDesign(Design design)
//    {
//        await Task.Delay(100);
//        return design;
//    }

//    public async Task<List<Design>> GetDesigns(int skip = 0, int take = 10)
//    {
//        await Task.Delay(100);
//        return new List<Design>();
//    }

//    public async Task<bool> LikeDesign(string designId)
//    {
//        await Task.Delay(100);
//        return true;
//    }

//    public async Task<bool> UpdateDesign(Design design)
//    {
//        await Task.Delay(100);
//        return true;
//    }
//}
