using System.Text;
using System.Text.Json;
using TshirtMaker.DTOs;
using TshirtMaker.Repositories.Interfaces;
using TshirtMaker.Services.Supabase;

namespace TshirtMaker.Repositories
{
    public class ShippingAddressRepository : BaseRepository<ShippingAddressDto>, IShippingAddressRepository
    {
        public ShippingAddressRepository(Supabase.Client supabaseClient, string supabaseUrl, string supabaseKey, ISupabaseAccessTokenProvider tokenProvider)
            : base(supabaseClient, "shipping_addresses", supabaseUrl, supabaseKey, tokenProvider)
        {
        }

        public async Task<IEnumerable<ShippingAddressDto>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?user_id=eq.{userId}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<ShippingAddressDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<ShippingAddressDto>();

            return items;
        }

        public async Task<ShippingAddressDto?> GetDefaultAddressAsync(Guid userId)
        {
            var response = await SendGetAsync($"/rest/v1/{_tableName}?user_id=eq.{userId}&is_default=eq.true");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<ShippingAddressDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return items?.FirstOrDefault();
        }

        public async Task<IEnumerable<ShippingAddressDto>> GetByCountryAsync(string countryCode, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?country_code=eq.{countryCode}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<ShippingAddressDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<ShippingAddressDto>();

            return items;
        }

        public async Task<bool> SetDefaultAddressAsync(Guid addressId, Guid userId)
        {
            try
            {
                var updateData = new { is_default = false };
                var json = JsonSerializer.Serialize(updateData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await SendPatchAsync($"/rest/v1/{_tableName}?user_id=eq.{userId}", content);

                var setDefaultData = new { is_default = true };
                var setDefaultJson = JsonSerializer.Serialize(setDefaultData);
                var setDefaultContent = new StringContent(setDefaultJson, Encoding.UTF8, "application/json");

                var response = await SendPatchAsync($"/rest/v1/{_tableName}?id=eq.{addressId}&user_id=eq.{userId}", setDefaultContent);
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
