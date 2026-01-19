using System.ComponentModel.DataAnnotations;
using TshirtMaker.Models.Core;
using TshirtMaker.Models.Enums;

namespace TshirtMaker.PublicData
{
    public class StylePreset : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public StylePresetType PresetType { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(2048)]
        public string ThumbnailUrl { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string PromptTemplate { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? DefaultNegativePrompt { get; set; }

        [MaxLength(4000)]
        public string? ExampleOutputs { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsFeatured { get; set; } = false;

        public int UsageCount { get; set; } = 0;

        public int DisplayOrder { get; set; } = 0;

        public string ApplyTemplate(string userPrompt)
        {
            if (string.IsNullOrEmpty(PromptTemplate))
                return userPrompt;

            return PromptTemplate.Replace("{prompt}", userPrompt);
        }

        public string[] GetExampleOutputsArray()
        {
            if (string.IsNullOrEmpty(ExampleOutputs))
                return Array.Empty<string>();

            return System.Text.Json.JsonSerializer.Deserialize<string[]>(ExampleOutputs) ?? Array.Empty<string>();
        }

        public void SetExampleOutputsArray(string[] urls)
        {
            ExampleOutputs = System.Text.Json.JsonSerializer.Serialize(urls);
        }

        public static List<StylePreset> GetDefaultPresets()
        {
            return new List<StylePreset>
            {
                new StylePreset
                {
                    Name = "Cyberpunk",
                    PresetType = StylePresetType.Cyberpunk,
                    Description = "Neon-lit futuristic aesthetics with tech noir vibes",
                    PromptTemplate = "{prompt}, cyberpunk style, neon colors, futuristic, tech noir, high contrast",
                    DefaultNegativePrompt = "blur, low quality, watermark, text",
                    IsActive = true,
                    IsFeatured = true,
                    DisplayOrder = 1
                },
                new StylePreset
                {
                    Name = "Anime",
                    PresetType = StylePresetType.Anime,
                    Description = "Japanese animation inspired artwork",
                    PromptTemplate = "{prompt}, anime style, manga, japanese animation, vibrant colors, cel shaded",
                    DefaultNegativePrompt = "realistic, photographic, blur, 3d render",
                    IsActive = true,
                    IsFeatured = true,
                    DisplayOrder = 2
                },
                new StylePreset
                {
                    Name = "Minimalism",
                    PresetType = StylePresetType.Minimalism,
                    Description = "Clean, simple designs with essential elements only",
                    PromptTemplate = "{prompt}, minimalist style, simple, clean lines, essential elements, negative space",
                    DefaultNegativePrompt = "complex, detailed, busy, cluttered",
                    IsActive = true,
                    IsFeatured = true,
                    DisplayOrder = 3
                },
                new StylePreset
                {
                    Name = "Oil Paint",
                    PresetType = StylePresetType.OilPaint,
                    Description = "Classical oil painting aesthetic",
                    PromptTemplate = "{prompt}, oil painting style, brush strokes, textured, artistic, classical art",
                    DefaultNegativePrompt = "digital, smooth, photographic, modern",
                    IsActive = true,
                    IsFeatured = true,
                    DisplayOrder = 4
                }
            };
        }
    }
}
