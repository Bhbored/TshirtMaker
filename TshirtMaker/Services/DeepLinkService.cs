namespace TshirtMaker.Services
{
    /// <summary>
    /// Service for generating deep links to social media platforms for sharing content
    /// </summary>
    public class DeepLinkService
    {
        // Website URL - editable for easy configuration
        private readonly string _websiteUrl = "https://clothiq.com"; // TODO: Update with your actual website URL

        // Default share text - editable for customization
        private readonly string _defaultShareText = "Check out my custom design on ClothIQ!"; // TODO: Customize share message

        /// <summary>
        /// Generates a Twitter/X share URL with the image and text
        /// </summary>
        /// <param name="imageUrl">URL of the image to share</param>
        /// <param name="customText">Optional custom text (uses default if not provided)</param>
        /// <returns>Twitter/X share URL</returns>
        public string GetTwitterShareUrl(string imageUrl, string? customText = null)
        {
            var text = customText ?? _defaultShareText;
            // Twitter/X share URL format
            var shareText = Uri.EscapeDataString($"{text} {_websiteUrl}");
            return $"https://twitter.com/intent/tweet?text={shareText}&url={Uri.EscapeDataString(imageUrl)}";
        }

        /// <summary>
        /// Generates a WhatsApp share URL with the image and text
        /// Note: WhatsApp uses a different format - opens chat with pre-filled message
        /// </summary>
        /// <param name="imageUrl">URL of the image to share</param>
        /// <param name="customText">Optional custom text (uses default if not provided)</param>
        /// <returns>WhatsApp share URL</returns>
        public string GetWhatsAppShareUrl(string imageUrl, string? customText = null)
        {
            var text = customText ?? _defaultShareText;
            // WhatsApp share URL format - opens chat with message
            var message = Uri.EscapeDataString($"{text} {imageUrl} {_websiteUrl}");
            return $"https://wa.me/?text={message}";
        }

        /// <summary>
        /// Generates an Instagram share URL
        /// Note: Instagram doesn't support direct URL sharing, this opens Instagram app
        /// </summary>
        /// <param name="imageUrl">URL of the image to share</param>
        /// <param name="customText">Optional custom text (uses default if not provided)</param>
        /// <returns>Instagram share URL</returns>
        public string GetInstagramShareUrl(string imageUrl, string? customText = null)
        {
            // Instagram doesn't support direct URL sharing via web
            // This opens Instagram app - user will need to manually share
            // Alternative: Use Instagram Basic Display API for authenticated sharing
            return "https://www.instagram.com/"; // TODO: Implement Instagram sharing if needed
        }

        /// <summary>
        /// Generates a Facebook share URL with the image and text
        /// </summary>
        /// <param name="imageUrl">URL of the image to share</param>
        /// <param name="customText">Optional custom text (uses default if not provided)</param>
        /// <returns>Facebook share URL</returns>
        public string GetFacebookShareUrl(string imageUrl, string? customText = null)
        {
            var text = customText ?? _defaultShareText;
            // Facebook share URL format
            var shareUrl = Uri.EscapeDataString(_websiteUrl);
            var quote = Uri.EscapeDataString(text);
            return $"https://www.facebook.com/sharer/sharer.php?u={shareUrl}&quote={quote}&picture={Uri.EscapeDataString(imageUrl)}";
        }
    }
}
