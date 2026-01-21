using System.Text.Json;
using TshirtMaker.DTOs;
using TshirtMaker.Repositories.Interfaces;
using TshirtMaker.Services.Supabase;

namespace TshirtMaker.Repositories
{
    public class OrderItemRepository : BaseRepository<OrderItemDto>, IOrderItemRepository
    {
        public OrderItemRepository(Supabase.Client supabaseClient, string supabaseUrl, string supabaseKey, ISupabaseAccessTokenProvider tokenProvider)
            : base(supabaseClient, "order_items", supabaseUrl, supabaseKey, tokenProvider)
        {
        }

        public async Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(Guid orderId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?order_id=eq.{orderId}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<OrderItemDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<OrderItemDto>();

            return items;
        }

        public async Task<IEnumerable<OrderItemDto>> GetByDesignIdAsync(Guid designId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?design_id=eq.{designId}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<OrderItemDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<OrderItemDto>();

            return items;
        }

        public async Task<decimal> GetOrderTotalAsync(Guid orderId)
        {
            var response = await SendGetAsync($"/rest/v1/{_tableName}?order_id=eq.{orderId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<OrderItemDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<OrderItemDto>();

            return items.Sum(item => item.TotalPrice);
        }

        public async Task<int> GetOrderItemCountAsync(Guid orderId)
        {
            var response = await SendGetAsync($"/rest/v1/{_tableName}?order_id=eq.{orderId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<OrderItemDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<OrderItemDto>();

            return items.Count;
        }
    }
}
