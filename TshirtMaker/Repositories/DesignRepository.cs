using System.Text.Json;
using TshirtMaker.DTOs;
using TshirtMaker.Repositories.Interfaces;

namespace TshirtMaker.Repositories
{
    public class DesignRepository : BaseRepository<DesignDto>, IDesignRepository
    {
        public DesignRepository(Supabase.Client supabaseClient, string supabaseUrl, string supabaseKey) 
            : base(supabaseClient, "designs", supabaseUrl, supabaseKey)
        {
        }

        public async Task<IEnumerable<DesignDto>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?user_id=eq.{userId}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<DesignDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<DesignDto>();

            return items;
        }

        public async Task<IEnumerable<DesignDto>> GetByClothingTypeAsync(int clothingType, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?clothing_type=eq.{clothingType}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<DesignDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<DesignDto>();

            return items;
        }

        public async Task<IEnumerable<DesignDto>> GetByMaterialAsync(string material, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?material=eq.{material}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<DesignDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<DesignDto>();

            return items;
        }

        public async Task<IEnumerable<DesignDto>> GetByStylePresetAsync(int stylePreset, int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await _httpClient.GetAsync($"/rest/v1/{_tableName}?style_preset=eq.{stylePreset}&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<DesignDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<DesignDto>();

            return items;
        }
    }
}