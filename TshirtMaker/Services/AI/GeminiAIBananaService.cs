using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TshirtMaker.Models.Enums;
using TshirtMaker.Services.AI;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace TshirtMaker.Services
{
    public class GeminiAIBananaService : IAIDesignService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string ApiModel = "gemini-2.5-flash-image";

        public string? ErrorMessage { get; private set; }

        public GeminiAIBananaService(IConfiguration configuration, HttpClient httpClient, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<string>> GenerateInitialDesignsAsync(string prompt, string? negativePrompt, StylePresetType? style)
        {
            ErrorMessage = null;

            var unsafeKeywords = new[] { "nudity", "violence", "hate speech", "gore", "explicit", "sexual" };
            string combinedPromptText = (prompt + " " + (negativePrompt ?? "")).ToLowerInvariant();
            if (unsafeKeywords.Any(keyword => combinedPromptText.Contains(keyword)))
            {
                ErrorMessage = "The prompt contains content that violates our safety guidelines. Please modify your request.";
                return new List<string>();
            }

            try
            {
                var apiKey = _configuration["Gemini:ApiKey"];
                if (string.IsNullOrEmpty(apiKey) || apiKey.Contains("PASTE_YOUR"))
                {
                    ErrorMessage = "Gemini API key is not configured in appsettings.json.";
                    return new List<string>();
                }

                string modifiedPrompt = $"Create 4 variations of the following prompt: {prompt}";

                if (style.HasValue)
                {
                    modifiedPrompt += $", in the style of {style.Value}";
                }

                if (!string.IsNullOrWhiteSpace(negativePrompt))
                {
                    modifiedPrompt += $", excluding {negativePrompt}";
                }

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = modifiedPrompt }
                            }
                        }
                    }
                };

                var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{ApiModel}:generateContent?key={apiKey}";

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var jsonContent = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"API call failed: {errorContent}";
                    return new List<string>();
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonNode.Parse(responseContent);

                var fullDestinationPath = Path.Combine(_webHostEnvironment.WebRootPath, "userImages", "temp");
                if (!Directory.Exists(fullDestinationPath))
                {
                    Directory.CreateDirectory(fullDestinationPath);
                }

                var localPaths = new List<string>();

                if (jsonResponse["candidates"] is JsonArray candidates)
                {
                    foreach (var candidate in candidates)
                    {
                        if (candidate["content"]?["parts"] is JsonArray parts)
                        {
                            foreach (var part in parts)
                            {
                                if (part["inlineData"]?["data"] is JsonValue dataValue &&
                                    part["inlineData"]?["mimeType"] is JsonValue mimeTypeValue)
                                {
                                    var base64Data = dataValue.ToString();
                                    var mimeType = mimeTypeValue.ToString();

                                    if (string.IsNullOrEmpty(base64Data) || string.IsNullOrEmpty(mimeType)) continue;

                                    var imageBytes = Convert.FromBase64String(base64Data);
                                    var fileExtension = mimeType.Split('/').LastOrDefault() ?? "png";
                                    var fileName = $"{Guid.NewGuid()}.{fileExtension}";
                                    var fullFilePath = Path.Combine(fullDestinationPath, fileName);

                                    await File.WriteAllBytesAsync(fullFilePath, imageBytes);
                                    localPaths.Add($"/userImages/temp/{fileName}");
                                }
                            }
                        }
                    }
                }

                if (localPaths.Count == 0)
                {
                    ErrorMessage = "API response was successful, but no valid inline image data was found.";
                    return new List<string>();
                }

                return localPaths;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                return new List<string>();
            }
        }

        public Task<string> FinalizeDesignAsync(string selectedDesignUrl, string clothingImageUrl, string color)
        {
            throw new NotImplementedException();
        }
    }
}