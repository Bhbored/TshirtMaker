namespace TshirtMaker.Models;

public class Design
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public ClothingType ClothingType { get; set; }
    public string Color { get; set; } = "#FFFFFF";
    public ClothingSize Size { get; set; }
    public Material Material { get; set; }
    public PrintPosition PrintPosition { get; set; }
    public string GeneratedImageUrl { get; set; } = string.Empty;
    public string FinalImageUrl { get; set; } = string.Empty;
    public int Likes { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsShared { get; set; } = false;
}
