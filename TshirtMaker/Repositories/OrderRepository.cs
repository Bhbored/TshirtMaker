using System.Text.Json;
using TshirtMaker.DTOs;
using TshirtMaker.Repositories.Interfaces;
using TshirtMaker.Services.Supabase;

namespace TshirtMaker.Repositories
{
    public class OrderRepository : BaseRepository<OrderDto>, IOrderRepository
    {
        public OrderRepository(Supabase.Client supabaseClient, string supabaseUrl, string supabaseKey, ISupabaseAccessTokenProvider tokenProvider)
            : base(supabaseClient, "orders", supabaseUrl, supabaseKey, tokenProvider)
        {
        }

        public async Task<IEnumerable<OrderDto>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?user_id=eq.{userId}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<OrderDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<OrderDto>();

            return items;
        }

        public async Task<IEnumerable<OrderDto>> GetByStatusAsync(int status, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?status=eq.{status}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<OrderDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<OrderDto>();

            return items;
        }

        public async Task<OrderDto?> GetByOrderNumberAsync(string orderNumber)
        {
            var response = await SendGetAsync($"/rest/v1/{_tableName}?order_number=eq.{orderNumber}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<OrderDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return items?.FirstOrDefault();
        }

        public async Task<IEnumerable<OrderDto>> GetRecentOrdersAsync(int count = 10)
        {
            var response = await SendGetAsync($"/rest/v1/{_tableName}?order=created_at.desc&limit={count}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<OrderDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<OrderDto>();

            return items;
        }

        public async Task<int> GetUserOrderCountAsync(Guid userId)
        {
            var response = await SendGetAsync($"/rest/v1/{_tableName}?user_id=eq.{userId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<OrderDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<OrderDto>();

            return items.Count;
        }
    }
}
