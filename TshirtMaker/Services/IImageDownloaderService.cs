namespace TshirtMaker.Services
{
    public interface IImageDownloaderService
    {
        Task<List<string>> DownloadAndSaveImagesAsync(List<string> imageUrls, string relativePath);
    }
}
