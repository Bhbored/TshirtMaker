using System.Text.Json;
using TshirtMaker.DTOs;
using TshirtMaker.Repositories.Interfaces;
using TshirtMaker.Services.Supabase;

namespace TshirtMaker.Repositories
{
    public class CollectionRepository : BaseRepository<CollectionDto>, ICollectionRepository
    {
        public CollectionRepository(Supabase.Client supabaseClient, string supabaseUrl, string supabaseKey, ISupabaseAccessTokenProvider tokenProvider)
            : base(supabaseClient, "collections", supabaseUrl, supabaseKey, tokenProvider)
        {
        }

        public async Task<IEnumerable<CollectionDto>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?user_id=eq.{userId}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<CollectionDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<CollectionDto>();

            return items;
        }

        public async Task<IEnumerable<CollectionDto>> GetByCopiedDesignIdAsync(Guid copiedDesignId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?copied_design_id=eq.{copiedDesignId}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<CollectionDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<CollectionDto>();

            return items;
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid copiedDesignId)
        {
            var response = await SendGetAsync($"/rest/v1/{_tableName}?user_id=eq.{userId}&copied_design_id=eq.{copiedDesignId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<CollectionDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<CollectionDto>();

            return items.Any();
        }
    }
}
