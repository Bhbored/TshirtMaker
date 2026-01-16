using System.ComponentModel.DataAnnotations;
using TshirtMaker.Models.Enums;

namespace TshirtMaker.Models.AI
{
    public class AIGenerationRequest
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Prompt is required")]
        [MaxLength(500, ErrorMessage = "Prompt cannot exceed 500 characters")]
        public string Prompt { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Negative prompt cannot exceed 500 characters")]
        public string? NegativePrompt { get; set; }

        public StylePresetType? StylePreset { get; set; }

        [Required]
        public ClothingType ClothingType { get; set; }

        [Required]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Color must be a valid hex code")]
        public string Color { get; set; } = "#FFFFFF";

        [Required]
        public ClothingSize Size { get; set; }

        [Required]
        public Material Material { get; set; }

        public PrintPosition PrintPosition { get; set; } = PrintPosition.Front;

        public Resolution Resolution { get; set; } = Resolution.TwoK;
        public int NumberOfImages { get; set; } = 6;
        public string? Seed { get; set; }
        public bool UsePromptEnhancement { get; set; } = true;

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public string? ClientIpAddress { get; set; }
        public string? UserAgent { get; set; }

        public bool IsValid(out List<string> errors)
        {
            errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Prompt))
                errors.Add("Prompt is required");

            if (Prompt?.Length > 500)
                errors.Add("Prompt is too long (max 500 characters)");

            if (NegativePrompt?.Length > 500)
                errors.Add("Negative prompt is too long (max 500 characters)");

            if (NumberOfImages < 1 || NumberOfImages > 10)
                errors.Add("Number of images must be between 1 and 10");

            return errors.Count == 0;
        }

        public string BuildEnhancedPrompt()
        {
            if (!UsePromptEnhancement)
                return Prompt;

            var position = PrintPosition == PrintPosition.Front ? "front" : "back";
            var clothingName = ClothingType.ToString().ToLower();

            var enhancedPrompt = $"Create a design for a {clothingName} print. " +
                               $"The design should be suitable for printing on the {position} of a {Color} garment. " +
                               $"Design request: {Prompt}. ";

            if (StylePreset.HasValue)
            {
                enhancedPrompt += $"Style: {StylePreset.Value}. ";
            }

            enhancedPrompt += "Make it modern, creative, and suitable for apparel printing with transparent or matching background.";

            return enhancedPrompt;
        }
    }
}
