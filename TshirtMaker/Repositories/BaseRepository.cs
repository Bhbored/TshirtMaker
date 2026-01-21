using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TshirtMaker.DTOs;
using TshirtMaker.Repositories.Interfaces;
using TshirtMaker.Services.Supabase;

namespace TshirtMaker.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntityDto, new()
    {
        protected readonly Supabase.Client _supabaseClient;
        protected readonly string _tableName;
        protected readonly HttpClient _httpClient;
        protected readonly string _supabaseUrl;
        protected readonly string _supabaseKey;
        private readonly ISupabaseAccessTokenProvider _tokenProvider;

        public BaseRepository(Supabase.Client supabaseClient, string tableName, string supabaseUrl, string supabaseKey, ISupabaseAccessTokenProvider tokenProvider)
        {
            _supabaseClient = supabaseClient;
            _tableName = tableName;
            _supabaseUrl = supabaseUrl;
            _supabaseKey = supabaseKey;
            _tokenProvider = tokenProvider;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(supabaseUrl)
            };
            _httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
        }

        private async Task SetAuthorizationAsync()
        {
            var token = await _tokenProvider.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token ?? _supabaseKey);
        }

        protected async Task<HttpResponseMessage> SendGetAsync(string requestUri)
        {
            await SetAuthorizationAsync();
            return await _httpClient.GetAsync(requestUri);
        }
        protected async Task<HttpResponseMessage> SendPostAsync(string requestUri, HttpContent content)
        {
            await SetAuthorizationAsync();
            return await _httpClient.PostAsync(requestUri, content);
        }
        protected async Task<HttpResponseMessage> SendPatchAsync(string requestUri, HttpContent content)
        {
            await SetAuthorizationAsync();
            return await _httpClient.PatchAsync(requestUri, content);
        }
        protected async Task<HttpResponseMessage> SendDeleteAsync(string requestUri)
        {
            await SetAuthorizationAsync();
            return await _httpClient.DeleteAsync(requestUri);
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            var response = await SendGetAsync($"/rest/v1/{_tableName}?id=eq.{id}&select=*");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<T>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return items?.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var offset = (pageNumber - 1) * pageSize;
            var response = await SendGetAsync($"/rest/v1/{_tableName}?select=*&order=created_at.desc&offset={offset}&limit={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<T>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<T>();

            return items;
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            var json = JsonSerializer.Serialize(entity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await SendPostAsync($"/rest/v1/{_tableName}", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var created = JsonSerializer.Deserialize<List<T>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return created?.FirstOrDefault() ?? entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;

            var json = JsonSerializer.Serialize(entity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await SendPatchAsync($"/rest/v1/{_tableName}?id=eq.{entity.Id}", content);
            response.EnsureSuccessStatusCode();

            return entity;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await SendDeleteAsync($"/rest/v1/{_tableName}?id=eq.{id}");
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
