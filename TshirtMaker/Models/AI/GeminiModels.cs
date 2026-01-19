using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TshirtMaker.Models.AI
{
    // Phase 1: Image Generation

    public class GeminiImageGenerationRequest
    {
        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = string.Empty;

        [JsonPropertyName("negative_prompt")]
        public string? NegativePrompt { get; set; }

        [JsonPropertyName("number_of_images")]
        public int NumberOfImages { get; set; } = 4;

        // Add other parameters like style, quality, etc. as needed by the API
    }

    public class GeminiImageGenerationResponse
    {
        [JsonPropertyName("images")]
        public List<ImageData> Images { get; set; } = new();
    }

    // Phase 2: Image Editing/Composition

    public class GeminiImageEditRequest
    {
        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = string.Empty;

        [JsonPropertyName("image")]
        public ImageData? Image { get; set; }

        [JsonPropertyName("mask")]
        public ImageData? Mask { get; set; }
    }

    public class GeminiImageEditResponse
    {
        [JsonPropertyName("image")]
        public ImageData? Image { get; set; }
    }

    // Common
    public class ImageData
    {
        [JsonPropertyName("b64_json")]
        public string? B64Json { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
