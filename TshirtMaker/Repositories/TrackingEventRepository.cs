using System.Text.Json;
using TshirtMaker.DTOs;
using TshirtMaker.Repositories.Interfaces;

namespace TshirtMaker.Repositories
{
    public class TrackingEventRepository : BaseRepository<TrackingEventDto>, ITrackingEventRepository
    {
        public TrackingEventRepository(Supabase.Client supabaseClient, string supabaseUrl, string supabaseKey) 
            : base(supabaseClient, "tracking_events", supabaseUrl, supabaseKey)
        {
        }

        public async Task<IEnumerable<TrackingEventDto>> GetByOrderIdAsync(Guid orderId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?order_id=eq.{orderId}&order=display_order.asc,created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<TrackingEventDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<TrackingEventDto>();

            return items;
        }

        public async Task<IEnumerable<TrackingEventDto>> GetActiveEventsByOrderIdAsync(Guid orderId)
        {
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?order_id=eq.{orderId}&is_active=eq.true&order=display_order.asc,created_at.desc");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<TrackingEventDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<TrackingEventDto>();

            return items;
        }

        public async Task<TrackingEventDto?> GetLastEventByOrderIdAsync(Guid orderId)
        {
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?order_id=eq.{orderId}&order=display_order.desc,created_at.desc&limit=1");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<TrackingEventDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return items?.FirstOrDefault();
        }

        public async Task<IEnumerable<TrackingEventDto>> GetByEventTypeAsync(string eventType, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?event_type=eq.{eventType}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<TrackingEventDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<TrackingEventDto>();

            return items;
        }

        public async Task<bool> SetEventInactiveAsync(Guid eventId)
        {
            var trackingEvent = await GetByIdAsync(eventId);
            if (trackingEvent == null) return false;

            trackingEvent.IsActive = false;
            await UpdateAsync(trackingEvent);
            return true;
        }
    }
}
