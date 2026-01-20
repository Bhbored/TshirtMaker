using System.Text;
using System.Text.Json;
using TshirtMaker.DTOs;
using TshirtMaker.Repositories.Interfaces;

namespace TshirtMaker.Repositories
{
    public class NotificationRepository : BaseRepository<NotificationDto>, INotificationRepository
    {
        public NotificationRepository(Supabase.Client supabaseClient, string supabaseUrl, string supabaseKey) 
            : base(supabaseClient, "notifications", supabaseUrl, supabaseKey)
        {
        }

        public async Task<IEnumerable<NotificationDto>> GetByRecipientIdAsync(Guid recipientId, int pageNumber = 1, int pageSize = 10, bool unreadOnly = false)
        {
            var offset = (pageNumber - 1) * pageSize;
            var url = unreadOnly 
                ? $"/rest/v1/{_tableName}?recipient_id=eq.{recipientId}&is_read=eq.false&order=created_at.desc&offset={offset}&limit={pageSize}"
                : $"/rest/v1/{_tableName}?recipient_id=eq.{recipientId}&order=created_at.desc&offset={offset}&limit={pageSize}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<NotificationDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<NotificationDto>();

            return items;
        }

        public async Task<int> GetUnreadCountAsync(Guid recipientId)
        {
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?recipient_id=eq.{recipientId}&is_read=eq.false");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<NotificationDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<NotificationDto>();

            return items.Count;
        }

        public async Task<bool> MarkAsReadAsync(Guid notificationId)
        {
            var notification = await GetByIdAsync(notificationId);
            if (notification == null) return false;

            notification.IsRead = true;
            await UpdateAsync(notification);
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(Guid recipientId)
        {
            var updateData = new { is_read = true };
            var json = JsonSerializer.Serialize(updateData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PatchAsync($"/rest/v1/{_tableName}?recipient_id=eq.{recipientId}&is_read=eq.false", content);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarkAsActionTakenAsync(Guid notificationId)
        {
            var notification = await GetByIdAsync(notificationId);
            if (notification == null) return false;

            notification.ActionTaken = true;
            await UpdateAsync(notification);
            return true;
        }
    }
}
