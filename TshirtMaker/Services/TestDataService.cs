using TshirtMaker.Models.Core;
using TshirtMaker.Models.Social;
using TshirtMaker.PublicData;
using TshirtMaker.Tests;

namespace TshirtMaker.Services;

public class TestDataService
{

    public List<User> GetAllUsers() => TestUsers.Users;


    public List<MaterialPreview> GetMaterialPresets() => PublicPresets.MaterialPresets;
    public List<StylePresetPreview> GetStylesPresets() => PublicPresets.StylePresetPreviews;
    public List<Design> GetAllDesigns() => TestUsers.Designs;
    public List<Design> GetUserDesigns(Guid id) => TestUsers.Designs.Where(x => x.UserId == id).ToList();

    public List<Post> GetAllPosts() => TestUsers.Posts;
    public List<Post> GetUserPosts(Guid id) => TestUsers.Posts.Where(x => x.PosterId == id).ToList();


}
