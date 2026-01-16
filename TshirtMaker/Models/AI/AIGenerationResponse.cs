using System.ComponentModel.DataAnnotations;

namespace TshirtMaker.Models.AI
{
    public class AIGenerationResponse
    {
        public Guid SessionId { get; set; }
        public bool Success { get; set; }

        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        public List<string> ImageUrls { get; set; } = new();

        public bool IsContentBlocked { get; set; } = false;

        [MaxLength(500)]
        public string? ContentBlockedReason { get; set; }

        [MaxLength(100)]
        public string? ModelUsed { get; set; }

        [MaxLength(50)]
        public string? ModelVersion { get; set; }

        public int? TokensUsed { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public TimeSpan? GenerationTime { get; set; }

        [MaxLength(50)]
        public string? ErrorCode { get; set; }

        [MaxLength(1000)]
        public string? ErrorDetails { get; set; }

        public decimal? EstimatedCost { get; set; }

        public string? CostCurrency { get; set; } = "USD";

        public int ImagesGenerated { get; set; }

        public int ImagesFailedModeration { get; set; }

        public static AIGenerationResponse CreateSuccess(List<string> imageUrls, string message = "Design generated successfully!")
        {
            return new AIGenerationResponse
            {
                Success = true,
                ImageUrls = imageUrls,
                Message = message,
                ImagesGenerated = imageUrls.Count,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public static AIGenerationResponse CreateError(string message, string? errorCode = null)
        {
            return new AIGenerationResponse
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                ErrorDetails = message,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public static AIGenerationResponse CreateContentBlocked(string reason)
        {
            return new AIGenerationResponse
            {
                Success = false,
                IsContentBlocked = true,
                ContentBlockedReason = reason,
                Message = "Content blocked: " + reason,
                GeneratedAt = DateTime.UtcNow
            };
        }
    }
}
