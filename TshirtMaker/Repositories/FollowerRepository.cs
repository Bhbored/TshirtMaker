using System.Text;
using System.Text.Json;
using TshirtMaker.DTOs;
using TshirtMaker.Repositories.Interfaces;
using TshirtMaker.Services.Supabase;

namespace TshirtMaker.Repositories
{
    public class FollowerRepository : BaseRepository<FollowerDto>, IFollowerRepository
    {
        public FollowerRepository(Supabase.Client supabaseClient, string supabaseUrl, string supabaseKey, ISupabaseAccessTokenProvider tokenProvider)
            : base(supabaseClient, "followers", supabaseUrl, supabaseKey, tokenProvider)
        {
        }

        public async Task<bool> FollowExistsAsync(Guid followerId, Guid followingId)
        {
            var response = await SendGetAsync($"/rest/v1/{_tableName}?follower_id=eq.{followerId}&following_id=eq.{followingId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<FollowerDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<FollowerDto>();

            return items.Any();
        }

        public async Task<IEnumerable<FollowerDto>> GetFollowersAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?following_id=eq.{userId}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<FollowerDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<FollowerDto>();

            return items;
        }

        public async Task<IEnumerable<FollowerDto>> GetFollowingAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?follower_id=eq.{userId}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<FollowerDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<FollowerDto>();

            return items;
        }

        public async Task<int> GetFollowerCountAsync(Guid userId)
        {
            var response = await SendGetAsync($"/rest/v1/{_tableName}?following_id=eq.{userId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<FollowerDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<FollowerDto>();

            return items.Count;
        }

        public async Task<int> GetFollowingCountAsync(Guid userId)
        {
            var response = await SendGetAsync($"/rest/v1/{_tableName}?follower_id=eq.{userId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<FollowerDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<FollowerDto>();

            return items.Count;
        }

        public async Task<bool> DeleteByFollowerAndFollowingAsync(Guid followerId, Guid followingId)
        {
            try
            {
                var response = await SendDeleteAsync($"/rest/v1/{_tableName}?follower_id=eq.{followerId}&following_id=eq.{followingId}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateMutualStatusAsync(Guid followerId, Guid followingId, bool isMutual)
        {
            var updateData = new { is_mutual = isMutual };
            var json = JsonSerializer.Serialize(updateData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await SendPatchAsync($"/rest/v1/{_tableName}?follower_id=eq.{followerId}&following_id=eq.{followingId}", content);
            response.EnsureSuccessStatusCode();

            if (isMutual)
            {
                var reverseResponse = await SendPatchAsync($"/rest/v1/{_tableName}?follower_id=eq.{followingId}&following_id=eq.{followerId}", content);
                reverseResponse.EnsureSuccessStatusCode();
            }

            return true;
        }
    }
}
