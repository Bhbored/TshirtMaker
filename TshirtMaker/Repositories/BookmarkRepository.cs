using System.Text.Json;
using TshirtMaker.DTOs;
using TshirtMaker.Repositories.Interfaces;
using TshirtMaker.Services.Supabase;

namespace TshirtMaker.Repositories
{
    public class BookmarkRepository : BaseRepository<BookmarkDto>, IBookmarkRepository
    {
        public BookmarkRepository(Supabase.Client supabaseClient, string supabaseUrl, string supabaseKey, ISupabaseAccessTokenProvider tokenProvider)
            : base(supabaseClient, "bookmarks", supabaseUrl, supabaseKey, tokenProvider)
        {
        }

        public async Task<bool> BookmarkExistsAsync(Guid userId, Guid postId)
        {
            var response = await SendGetAsync($"/rest/v1/{_tableName}?user_id=eq.{userId}&post_id=eq.{postId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<BookmarkDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<BookmarkDto>();

            return items.Any();
        }

        public async Task<IEnumerable<BookmarkDto>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?user_id=eq.{userId}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<BookmarkDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<BookmarkDto>();

            return items;
        }

        public async Task<IEnumerable<BookmarkDto>> GetByPostIdAsync(Guid postId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?post_id=eq.{postId}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<BookmarkDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<BookmarkDto>();

            return items;
        }

        public async Task<int> GetPostBookmarksCountAsync(Guid postId)
        {
            var response = await SendGetAsync($"/rest/v1/{_tableName}?post_id=eq.{postId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<BookmarkDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<BookmarkDto>();

            return items.Count;
        }

        public async Task<bool> DeleteByUserAndPostAsync(Guid userId, Guid postId)
        {
            try
            {
                var response = await SendDeleteAsync($"/rest/v1/{_tableName}?user_id=eq.{userId}&post_id=eq.{postId}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
