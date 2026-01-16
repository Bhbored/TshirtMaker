using TshirtMaker.Models.Core;
using TshirtMaker.Tests;

namespace TshirtMaker.Services;

public class TestDataService
{

    public List<User> GetAllUsers() => TestUsers.Users;

    public List<Design> GetAllDesigns() => TestUsers.Designs;
    public List<Design> GetUserDesigns(Guid id) => TestUsers.Designs.Where(x => x.UserId == id).ToList();


}
