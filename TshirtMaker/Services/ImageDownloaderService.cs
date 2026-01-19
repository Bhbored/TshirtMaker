using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TshirtMaker.Services
{
    public class ImageDownloaderService : IImageDownloaderService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly HttpClient _httpClient;

        public ImageDownloaderService(IWebHostEnvironment webHostEnvironment, HttpClient httpClient)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpClient = httpClient;
        }

        public async Task<List<string>> DownloadAndSaveImagesAsync(List<string> imageUrls, string relativePath)
        {
            var savedImagePaths = new List<string>();
            var rootPath = _webHostEnvironment.WebRootPath;
            var fullDestinationPath = Path.Combine(rootPath, relativePath);

            if (!Directory.Exists(fullDestinationPath))
            {
                Directory.CreateDirectory(fullDestinationPath);
            }

            foreach (var imageUrl in imageUrls)
            {
                try
                {
                    var response = await _httpClient.GetAsync(imageUrl);
                    response.EnsureSuccessStatusCode();
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    
                    var fileName = $"{Guid.NewGuid()}.png";
                    var fullFilePath = Path.Combine(fullDestinationPath, fileName);

                    await File.WriteAllBytesAsync(fullFilePath, imageBytes);

                    var webAccessiblePath = $"/{relativePath.Replace('\\', '/')}/{fileName}";
                    savedImagePaths.Add(webAccessiblePath);
                }
                catch (HttpRequestException ex)
                {
                    // Log the exception or handle it as needed
                    // For now, we'll just re-throw to be caught by the calling service
                    throw new Exception($"Failed to download image from {imageUrl}. Error: {ex.Message}", ex);
                }
            }

            return savedImagePaths;
        }
    }
}
