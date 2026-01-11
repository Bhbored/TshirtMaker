namespace TshirtMaker.Models.AI;

public class DesignRequestDto
{
    public string Prompt { get; set; } = string.Empty;
    public ClothingType ClothingType { get; set; }
    public string Color { get; set; } = string.Empty;
    public PrintPosition PrintPosition { get; set; }
}

public class DesignResponseDto
{
    public bool Success { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsContentBlocked { get; set; } = false;
}
