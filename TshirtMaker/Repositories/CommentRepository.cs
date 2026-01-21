using System.Text.Json;
using TshirtMaker.DTOs;
using TshirtMaker.Repositories.Interfaces;
using TshirtMaker.Services.Supabase;

namespace TshirtMaker.Repositories
{
    public class CommentRepository : BaseRepository<CommentDto>, ICommentRepository
    {
        public CommentRepository(Supabase.Client supabaseClient, string supabaseUrl, string supabaseKey, ISupabaseAccessTokenProvider tokenProvider)
            : base(supabaseClient, "comments", supabaseUrl, supabaseKey, tokenProvider)
        {
        }

        public async Task<IEnumerable<CommentDto>> GetByPostIdAsync(Guid postId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?post_id=eq.{postId}&order=created_at.asc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<CommentDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<CommentDto>();

            return items;
        }

        public async Task<IEnumerable<CommentDto>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?user_id=eq.{userId}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<CommentDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<CommentDto>();

            return items;
        }

        public async Task<int> GetPostCommentsCountAsync(Guid postId)
        {
            var response = await SendGetAsync($"/rest/v1/{_tableName}?post_id=eq.{postId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<CommentDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<CommentDto>();

            return items.Count;
        }
    }
}
