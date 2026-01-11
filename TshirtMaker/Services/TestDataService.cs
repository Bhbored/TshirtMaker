using TshirtMaker.Models;

namespace TshirtMaker.Services;

public class TestDataService
{
    private readonly List<User> _users;
    private readonly List<Design> _designs;
    private User _currentUser;

    public TestDataService()
    {
        _users = GenerateUsers();
        _currentUser = _users[0];
        _designs = GenerateDesigns();
    }

    public User GetCurrentUser() => _currentUser;

    public void SetCurrentUser(User user) => _currentUser = user;

    public void Logout() => _currentUser = _users[0];

    public List<User> GetAllUsers() => _users;

    public List<Design> GetAllDesigns() => _designs.Where(d => d.IsShared).ToList();

    public List<Design> GetUserDesigns(string userId) => _designs.Where(d => d.UserId == userId).ToList();

    public List<Design> GetTrendingDesigns(int count = 5) => 
        _designs.Where(d => d.IsShared)
                .OrderByDescending(d => d.Likes)
                .Take(count)
                .ToList();

    public List<Design> GetDesignsByFilter(string filter, int skip = 0, int take = 10)
    {
        var query = _designs.Where(d => d.IsShared);
        
        query = filter.ToLower() switch
        {
            "latest" => query.OrderByDescending(d => d.CreatedAt),
            "likes" => query.OrderByDescending(d => d.Likes),
            _ => query.OrderByDescending(d => d.Likes)
        };

        return query.Skip(skip).Take(take).ToList();
    }

    public void AddDesign(Design design)
    {
        _designs.Insert(0, design);
    }

    public void LikeDesign(string designId)
    {
        var design = _designs.FirstOrDefault(d => d.Id == designId);
        if (design != null)
        {
            design.Likes++;
        }
    }

    public void ShareDesign(string designId)
    {
        var design = _designs.FirstOrDefault(d => d.Id == designId);
        if (design != null)
        {
            design.IsShared = true;
        }
    }

    private List<User> GenerateUsers()
    {
        return new List<User>
        {
            new User { Id = "1", Username = "DesignPro", Email = "design@test.com", AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=DesignPro" },
            new User { Id = "2", Username = "ArtistX", Email = "artist@test.com", AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=ArtistX" },
            new User { Id = "3", Username = "CreativeMin", Email = "creative@test.com", AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=CreativeMin" },
            new User { Id = "4", Username = "StyleMaster", Email = "style@test.com", AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=StyleMaster" },
            new User { Id = "5", Username = "TrendSetter", Email = "trend@test.com", AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=TrendSetter" }
        };
    }

    private List<Design> GenerateDesigns()
    {
        var designs = new List<Design>();
        var random = new Random();
        var prompts = new[]
        {
            "Cyberpunk cityscape with neon lights",
            "Abstract geometric patterns in purple and gold",
            "Minimalist mountain landscape",
            "Retro 80s sunset with palm trees",
            "Japanese wave art in modern style",
            "Cosmic nebula with stars",
            "Graffiti style urban art",
            "Watercolor flowers bouquet",
            "Pixel art space invaders",
            "Hand-drawn coffee illustration"
        };

        for (int i = 0; i < 150; i++)
        {
            var user = _users[random.Next(_users.Count)];
            designs.Add(new Design
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Username = user.Username,
                UserAvatar = user.AvatarUrl,
                Prompt = prompts[random.Next(prompts.Length)],
                ClothingType = (ClothingType)random.Next(Enum.GetValues<ClothingType>().Length),
                Color = new[] { "#FFFFFF", "#000000", "#FF5733", "#3357FF", "#33FF57", "#FF00FF", "#00FFFF", "#FFFF00" }[random.Next(8)],
                Size = (ClothingSize)random.Next(Enum.GetValues<ClothingSize>().Length),
                Material = (Material)random.Next(Enum.GetValues<Material>().Length),
                PrintPosition = (PrintPosition)random.Next(2),
                GeneratedImageUrl = $"https://picsum.photos/400/400?random={i}",
                FinalImageUrl = $"https://picsum.photos/800/800?random={i}",
                Likes = random.Next(0, 2000),
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 60)),
                IsShared = true
            });
        }

        return designs;
    }
}
