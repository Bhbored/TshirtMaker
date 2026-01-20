using System.Text.Json;
using TshirtMaker.DTOs;
using TshirtMaker.Repositories.Interfaces;

namespace TshirtMaker.Repositories
{
    public class UserRepository : BaseRepository<UserDto>, IUserRepository
    {
        public UserRepository(Supabase.Client supabaseClient, string supabaseUrl, string supabaseKey)
            : base(supabaseClient, "users", supabaseUrl, supabaseKey)
        {
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?username=eq.{username}&select=*");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<UserDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return items?.FirstOrDefault();
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?email=eq.{email}&select=*");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<UserDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return items?.FirstOrDefault();
        }

        public async Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;

            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?or=(username.ilike.%{searchTerm}%,bio.ilike.%{searchTerm}%)&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<UserDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<UserDto>();

            return items;
        }

        public async Task<int> GetFollowersCountAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"/rest/v1/followers?following_user=eq.{userId}&select=count");
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
            var response = await _httpClient.GetAsync($"/rest/v1/followers?follower_id=eq.{userId}&select=count");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<FollowerDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<FollowerDto>();

            return items.Count;
        }
    }
}