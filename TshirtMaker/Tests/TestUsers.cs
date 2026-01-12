using TshirtMaker.Models;

namespace TshirtMaker.Tests
{
    public static class TestUsers
    {
        public static List<User> Users = [
             new User { Username = "DesignPro", Email = "design@test.com", Password = "123456", AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=DesignPro" },
            new User { Username = "ArtistX", Email = "artist@test.com", Password = "123456", AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=ArtistX" },
            new User { Username = "CreativeMin", Email = "creative@test.com", Password = "123456", AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=CreativeMin" },
            new User { Username = "StyleMaster", Email = "style@test.com", Password = "123456", AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=StyleMaster" },
            new User { Username = "TrendSetter", Email = "trend@test.com", Password = "123456", AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=TrendSetter" },
        ];
    }
}
