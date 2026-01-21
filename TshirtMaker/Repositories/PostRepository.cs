using System.Text.Json;
using TshirtMaker.DTOs;
using TshirtMaker.Repositories.Interfaces;

namespace TshirtMaker.Repositories
{
    /// <summary>
    /// PostRepository inherits from BaseRepository&lt;PostDto&gt;, which provides:
    /// - GetAllAsync(pageNumber, pageSize) — fetches ALL posts from the posts table (paginated, ordered by created_at desc)
    /// - GetByIdAsync(id)
    /// - CreateAsync(entity) — add
    /// - UpdateAsync(entity) — update
    /// - DeleteAsync(id) — delete
    /// </summary>
    public class PostRepository : BaseRepository<PostDto>, IPostRepository
    {
        public PostRepository(Supabase.Client supabaseClient, string supabaseUrl, string supabaseKey)
            : base(supabaseClient, "posts", supabaseUrl, supabaseKey)
        {
        }

        public async Task<IEnumerable<PostDto>> GetAllPostsAsync()
        {
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?select=*&order=created_at.desc");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<PostDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<PostDto>();

            return items;
        }

        public async Task<IEnumerable<PostDto>> GetByPosterIdAsync(Guid posterId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?poster_id=eq.{posterId}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<PostDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<PostDto>();

            return items;
        }

        public async Task<IEnumerable<PostDto>> GetByDesignIdAsync(Guid designId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?design_id=eq.{designId}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<PostDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<PostDto>();

            return items;
        }

        public async Task<IEnumerable<PostDto>> GetFeedAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;

            var followerResponse = await _httpClient.GetAsync($"/rest/v1/followers?follower_id=eq.{userId}&select=following_id");
            followerResponse.EnsureSuccessStatusCode();

            var followerContent = await followerResponse.Content.ReadAsStringAsync();
            var followers = JsonSerializer.Deserialize<List<FollowerDto>>(followerContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<FollowerDto>();

            var followingIds = followers.Select(f => f.FollowingId).ToList();

            if (!followingIds.Any())
            {
                return new List<PostDto>();
            }

            var idsParam = string.Join(",", followingIds.Select(id => $"\"{id}\""));
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?poster_id=in.({idsParam})&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<PostDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<PostDto>();

            return items;
        }

        public async Task<IEnumerable<PostDto>> GetTrendingAsync(int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?order=likes_count.desc,created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<PostDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<PostDto>();

            return items;
        }

        public async Task<IEnumerable<PostDto>> GetLatestAsync(int count = 12, int startIndex = 0)
        {
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?order=created_at.desc&offset={startIndex}&limit={count}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<PostDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<PostDto>();

            return items;
        }

        public async Task<int> GetTotalPostsCountAsync()
        {
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?select=count");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<PostDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<PostDto>();

            return items.Count;
        }
        public async Task<bool> ToggleAllowRemixAsync(Guid postId, bool allowRemix)
        {
            var post = await GetByIdAsync(postId);
            if (post == null) return false;

            post.AllowRemix = allowRemix;
            await UpdateAsync(post);
            return true;
        }
    }
}
