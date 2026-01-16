//using System.Text.Json;
//using System.Text;
//using TshirtMaker.Models.AI;

//namespace TshirtMaker.Services.AI;

///// <summary>
///// AI Design Service for generating custom t-shirt designs using OpenAI's DALL-E API.
///// This service handles image generation with content moderation to prevent unethical or 18+ content.
///// </summary>
//public class AIDesignService
//{
//    private readonly HttpClient _httpClient;
//    private readonly string _apiKey;
//    private readonly string _apiEndpoint = "https://api.openai.com/v1/images/generations";

//    /// <summary>
//    /// Initializes the AI Design Service with the OpenAI API key from configuration.
//    /// The API key should be stored in appsettings.json under "OpenAI:ApiKey".
//    /// </summary>
//    public AIDesignService(HttpClient httpClient, IConfiguration configuration)
//    {
//        _httpClient = httpClient;
//        _apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;
        
//        if (!string.IsNullOrEmpty(_apiKey))
//        {
//            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
//        }
//    }

//    /// <summary>
//    /// Generates a custom design based on the user's prompt with content moderation.
//    /// This method:
//    /// 1. Checks the prompt for inappropriate content
//    /// 2. If safe, sends a request to DALL-E to generate an image
//    /// 3. Returns the generated image URL or an error message
//    /// </summary>
//    /// <param name="request">The design request containing prompt and clothing details</param>
//    /// <returns>A response containing the image URL or error message</returns>
//    public async Task<DesignResponseDto> GenerateDesign(DesignRequestDto request)
//    {
//        if (string.IsNullOrEmpty(_apiKey))
//        {
//            return new DesignResponseDto
//            {
//                Success = false,
//                Message = "API key not configured. Please add OpenAI:ApiKey to appsettings.json"
//            };
//        }

//        if (!IsPromptSafe(request.Prompt))
//        {
//            return new DesignResponseDto
//            {
//                Success = false,
//                IsContentBlocked = true,
//                Message = "We appreciate your creativity, but this request doesn't align with our content guidelines. Please try a different design idea that's appropriate for all audiences."
//            };
//        }

//        var enhancedPrompt = EnhancePromptForClothing(request);

//        var requestBody = new
//        {
//            model = "dall-e-3",
//            prompt = enhancedPrompt,
//            n = 1,
//            size = "1024x1024",
//            quality = "standard"
//        };

//        try
//        {
//            var json = JsonSerializer.Serialize(requestBody);
//            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
//            var response = await _httpClient.PostAsync(_apiEndpoint, content);
            
//            if (!response.IsSuccessStatusCode)
//            {
//                var error = await response.Content.ReadAsStringAsync();
//                return new DesignResponseDto
//                {
//                    Success = false,
//                    Message = $"API Error: {response.StatusCode}"
//                };
//            }

//            var responseJson = await response.Content.ReadAsStringAsync();
//            var result = JsonSerializer.Deserialize<OpenAIImageResponse>(responseJson);

//            if (result?.Data != null && result.Data.Length > 0)
//            {
//                return new DesignResponseDto
//                {
//                    Success = true,
//                    ImageUrl = result.Data[0].Url,
//                    Message = "Design generated successfully!"
//                };
//            }

//            return new DesignResponseDto
//            {
//                Success = false,
//                Message = "No image was generated. Please try again."
//            };
//        }
//        catch (Exception ex)
//        {
//            return new DesignResponseDto
//            {
//                Success = false,
//                Message = $"Error generating design: {ex.Message}"
//            };
//        }
//    }

//    /// <summary>
//    /// Checks if the prompt contains inappropriate content.
//    /// This is a basic content filter that checks for explicit keywords.
//    /// In production, you should use OpenAI's Moderation API for better accuracy.
//    /// </summary>
//    private bool IsPromptSafe(string prompt)
//    {
//        var lowerPrompt = prompt.ToLower();
        
//        var bannedKeywords = new[]
//        {
//            "nude", "naked", "nsfw", "porn", "xxx", "sexual", "explicit",
//            "violence", "gore", "blood", "death", "kill", "weapon", "gun",
//            "hate", "racist", "discrimination", "offensive"
//        };

//        return !bannedKeywords.Any(keyword => lowerPrompt.Contains(keyword));
//    }

//    /// <summary>
//    /// Enhances the user's prompt to better suit clothing design generation.
//    /// Adds context about the clothing type, color, and print position.
//    /// </summary>
//    private string EnhancePromptForClothing(DesignRequestDto request)
//    {
//        var position = request.PrintPosition == Models.PrintPosition.Front ? "front" : "back";
//        return $"Create a design for a {request.ClothingType.ToString().ToLower()} print. " +
//               $"The design should be suitable for printing on the {position} of a {request.Color} garment. " +
//               $"Design request: {request.Prompt}. " +
//               $"Make it modern, creative, and suitable for apparel printing with transparent or matching background.";
//    }

//    private class OpenAIImageResponse
//    {
//        public long Created { get; set; }
//        public ImageData[]? Data { get; set; }
//    }

//    private class ImageData
//    {
//        public string Url { get; set; } = string.Empty;
//    }
//}
