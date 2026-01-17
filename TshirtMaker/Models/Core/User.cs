using System.ComponentModel.DataAnnotations;
using TshirtMaker.Models.Social;

namespace TshirtMaker.Models.Core
{
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Bio { get; set; }

        [MaxLength(2048)]
        public string AvatarUrl { get; set; } = string.Empty;

        [MaxLength(2048)]
        public string? CoverImageUrl { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }

        [MaxLength(2048)]
        public string? WebsiteUrl { get; set; }

        public int TotalDesigns { get; set; } = 0;

        public int TotalLikes { get; set; } = 0;
        public int TotalPosts { get; set; } = 0;
        public int TotalShares { get; set; } = 0;

        public int FollowersCount { get; set; } = 0;

        public int FollowingCount { get; set; } = 0;

        public DateTime? LastLoginAt { get; set; }

        public virtual ICollection<Design>? Designs { get; set; } = [];
        public virtual ICollection<Post>? Posts { get; set; } = [];
        public virtual ICollection<Like>? LikesGiven { get; set; } = [];
        public virtual ICollection<Like>? LikesTaken { get; set; } = [];

        public virtual ICollection<Comment>? Comments { get; set; } = [];
        public virtual ICollection<Bookmark>? Bookmarks { get; set; } = [];
        public virtual ICollection<Follower>? Followers { get; set; } = [];
        public virtual ICollection<Notification>? Notifications { get; set; } = [];
    }
}

