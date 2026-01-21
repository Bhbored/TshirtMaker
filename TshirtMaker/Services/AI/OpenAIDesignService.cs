using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TshirtMaker.Models.Enums;
using TshirtMaker.Services.AI;
using TshirtMaker.Services.Supabase;

namespace TshirtMaker.Services
{

    //never use openai key for image generation it's too expensive and u can't go lower than 1024x1024 switch to gemini later 0_0

    //read the docs before switching 
    public class OpenAIDesignService : IAIDesignService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly SupabaseStorageService _storageService;

        private const string ApiUrl = "https://api.openai.com/v1/images/generations";
        private const string ApiModel = "gpt-image-1";

        public string? ErrorMessage { get; private set; }

        public OpenAIDesignService(
            IConfiguration configuration,
            HttpClient httpClient,
            SupabaseStorageService storageService)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _storageService = storageService;
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        public async Task<List<string>> GenerateInitialDesignsAsync(
            string prompt,
            string? negativePrompt,
            StylePresetType? style,
            Guid userId,
            CancellationToken cancellationToken = default)
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

                var apiKey = _configuration["OpenAI:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    ErrorMessage = "OpenAI API key is not configured in appsettings.json.";
                    return new List<string>();
                }

                string modifiedPrompt = $"Create 1 variation of the following prompt: {prompt}";

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
                    model = ApiModel,
                    prompt = modifiedPrompt,
                    n = 1,
                    size = "auto"
                };

                var json = JsonSerializer.Serialize(requestBody);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey);


                var response = await _httpClient.PostAsync(ApiUrl, httpContent, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"OpenAI API call failed: {error}";
                    return new List<string>();
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(responseContent);

                var uploadedUrls = new List<string>();
                int currentIndex = await _storageService.GetNextTempImageIndexAsync(userId, cancellationToken);

                if (document.RootElement.TryGetProperty("data", out var dataArray))
                {
                    foreach (var item in dataArray.EnumerateArray())
                    {
                        if (!item.TryGetProperty("b64_json", out var b64Property))
                            continue;

                        var base64 = b64Property.GetString();
                        if (string.IsNullOrWhiteSpace(base64))
                            continue;

                        var imageBytes = Convert.FromBase64String(base64);
                        var fileName = $"temp_{currentIndex}.png";

                        var uploadedUrl = await _storageService.UploadToUserTempFolderAsync(
                            imageBytes,
                            fileName,
                            userId,
                            cancellationToken);

                        uploadedUrls.Add(uploadedUrl);
                        currentIndex++;
                    }
                }

                if (uploadedUrls.Count == 0)
                {
                    ErrorMessage = "API response was successful, but no image data was returned.";
                    return new List<string>();
                }

                return uploadedUrls;
            }
            catch (TaskCanceledException)
            {
                ErrorMessage = "The request timed out. Please try again or reduce the number of images.";
                return new List<string>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                return new List<string>();
            }
        }


        public async Task<string> FinalizeDesignAsync(string selectedDesignUrl, string clothingImageUrl, Guid userId, string color, CancellationToken cancellationToken = default)
        {
            ErrorMessage = null;

            try
            {
                var designResponse = await _httpClient.GetAsync(selectedDesignUrl, cancellationToken);
                if (!designResponse.IsSuccessStatusCode)
                {
                    ErrorMessage = $"Failed to download design image: {designResponse.StatusCode}";
                    return string.Empty;
                }

                var designBytes = await designResponse.Content.ReadAsByteArrayAsync(cancellationToken);

                byte[] clothingBytes;
                if (clothingImageUrl.StartsWith("http://") || clothingImageUrl.StartsWith("https://"))
                {
                    var clothingResponse = await _httpClient.GetAsync(clothingImageUrl, cancellationToken);
                    if (!clothingResponse.IsSuccessStatusCode)
                    {
                        ErrorMessage = $"Failed to download clothing image: {clothingResponse.StatusCode}";
                        return string.Empty;
                    }
                    clothingBytes = await clothingResponse.Content.ReadAsByteArrayAsync(cancellationToken);
                }
                else
                {
                    ErrorMessage = $"Invalid clothing image URL: {clothingImageUrl}";
                    return string.Empty;
                }

                string prompt = "Print the provided design naturally on the clothing piece.";
                if (!string.IsNullOrWhiteSpace(color) && !color.Equals("white", StringComparison.OrdinalIgnoreCase))
                    prompt += $" Make the clothing piece {color}.";
                prompt += " The final image should look like a professional studio shot with realistic lighting, shadows, and perspective.";

                var apiKey = _configuration["OpenAI:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    ErrorMessage = "OpenAI API key is not configured.";
                    return string.Empty;
                }

                using var multipart = new MultipartFormDataContent();
                multipart.Add(new ByteArrayContent(clothingBytes), "image", "clothing.png");
                multipart.Add(new ByteArrayContent(designBytes), "image", "design.png");
                multipart.Add(new StringContent(prompt), "prompt");
                multipart.Add(new StringContent(ApiModel), "model");
                multipart.Add(new StringContent("auto"), "size");
                multipart.Add(new StringContent("1"), "n");

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/images/edits")
                {
                    Content = multipart
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var response = await _httpClient.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync(cancellationToken);
                    ErrorMessage = $"OpenAI API call failed: {error}";
                    return string.Empty;
                }

                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                using var document = JsonDocument.Parse(responseJson);

                if (!document.RootElement.TryGetProperty("data", out var dataArray) || dataArray.GetArrayLength() == 0)
                {
                    ErrorMessage = "No image returned from OpenAI.";
                    return string.Empty;
                }

                var b64Image = dataArray[0].GetProperty("b64_json").GetString();
                if (string.IsNullOrWhiteSpace(b64Image))
                {
                    ErrorMessage = "OpenAI returned empty image data.";
                    return string.Empty;
                }

                int nextIndex = await _storageService.GetNextFinalImageIndexAsync(userId, cancellationToken);
                var finalFileName = $"final_{nextIndex}.png";
                var finalBytes = Convert.FromBase64String(b64Image);

                var uploadedUrl = await _storageService.UploadToUserFinalFolderAsync(
                    finalBytes,
                    finalFileName,
                    userId,
                    cancellationToken);

                return uploadedUrl;
            }
            catch (TaskCanceledException)
            {
                ErrorMessage = "The request timed out. Please try again.";
                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                return string.Empty;
            }
        }

    }
}
