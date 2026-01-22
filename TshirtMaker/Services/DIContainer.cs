using Supabase;
using TshirtMaker.Repositories;
using TshirtMaker.Repositories.Interfaces;
using TshirtMaker.Services.AI;
using TshirtMaker.Services.Supabase;
using System.Net.Http.Headers;

namespace TshirtMaker.Services
{
    public static class DIContainer
    {
        public static IServiceCollection RegisterSupabase(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase:Url not configured");
            var key = configuration["Supabase:Key"] ?? throw new InvalidOperationException("Supabase:Key not configured");

            services.AddSingleton<global::Supabase.Client>(sp =>
            {
                var options = new SupabaseOptions
                {
                    AutoRefreshToken = true,
                    AutoConnectRealtime = false,
                    SessionHandler = new SupabaseSessionHandler()
                };

                var client = new Client(url, key, options);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await client.InitializeAsync();
                    }
                    catch (Exception ex)
                    {
                        var logger = sp.GetService<ILoggerFactory>()?.CreateLogger("Supabase.Client.Init");
                        logger?.LogError(ex, "Failed to initialize Supabase.Client");
                    }
                });

                return client;
            });

            services.AddHttpClient("supabase", client =>
            {
                client.BaseAddress = new Uri(url);
                if (!client.DefaultRequestHeaders.Contains("apikey"))
                    client.DefaultRequestHeaders.Add("apikey", key);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(100);
            });

            return services;
        }

        public static IServiceCollection RegisterRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserRepository>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("supabase");
                var tokenProvider = sp.GetRequiredService<ISupabaseAccessTokenProvider>();
                var apiKey = configuration["Supabase:Key"]!;
                return new UserRepository(client, apiKey, tokenProvider);
            });

            services.AddScoped<IDesignRepository>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("supabase");
                var tokenProvider = sp.GetRequiredService<ISupabaseAccessTokenProvider>();
                var apiKey = configuration["Supabase:Key"]!;
                return new DesignRepository(client, apiKey, tokenProvider);
            });

            services.AddScoped<IPostRepository>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("supabase");
                var tokenProvider = sp.GetRequiredService<ISupabaseAccessTokenProvider>();
                var apiKey = configuration["Supabase:Key"]!;
                return new PostRepository(client, apiKey, tokenProvider);
            });

            services.AddScoped<ICommentRepository>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("supabase");
                var tokenProvider = sp.GetRequiredService<ISupabaseAccessTokenProvider>();
                var apiKey = configuration["Supabase:Key"]!;
                return new CommentRepository(client, apiKey, tokenProvider);
            });

            services.AddScoped<ILikeRepository>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("supabase");
                var tokenProvider = sp.GetRequiredService<ISupabaseAccessTokenProvider>();
                var apiKey = configuration["Supabase:Key"]!;
                return new LikeRepository(client, apiKey, tokenProvider);
            });

            services.AddScoped<IBookmarkRepository>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("supabase");
                var tokenProvider = sp.GetRequiredService<ISupabaseAccessTokenProvider>();
                var apiKey = configuration["Supabase:Key"]!;
                return new BookmarkRepository(client, apiKey, tokenProvider);
            });

            services.AddScoped<IFollowerRepository>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("supabase");
                var tokenProvider = sp.GetRequiredService<ISupabaseAccessTokenProvider>();
                var apiKey = configuration["Supabase:Key"]!;
                return new FollowerRepository(client, apiKey, tokenProvider);
            });

            services.AddScoped<INotificationRepository>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("supabase");
                var tokenProvider = sp.GetRequiredService<ISupabaseAccessTokenProvider>();
                var apiKey = configuration["Supabase:Key"]!;
                return new NotificationRepository(client, apiKey, tokenProvider);
            });

            services.AddScoped<ICollectionRepository>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("supabase");
                var tokenProvider = sp.GetRequiredService<ISupabaseAccessTokenProvider>();
                var apiKey = configuration["Supabase:Key"]!;
                return new CollectionRepository(client, apiKey, tokenProvider);
            });

            services.AddScoped<IOrderRepository>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("supabase");
                var tokenProvider = sp.GetRequiredService<ISupabaseAccessTokenProvider>();
                var apiKey = configuration["Supabase:Key"]!;
                return new OrderRepository(client, apiKey, tokenProvider);
            });

            services.AddScoped<IOrderItemRepository>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("supabase");
                var tokenProvider = sp.GetRequiredService<ISupabaseAccessTokenProvider>();
                var apiKey = configuration["Supabase:Key"]!;
                return new OrderItemRepository(client, apiKey, tokenProvider);
            });

            services.AddScoped<IShippingAddressRepository>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("supabase");
                var tokenProvider = sp.GetRequiredService<ISupabaseAccessTokenProvider>();
                var apiKey = configuration["Supabase:Key"]!;
                return new ShippingAddressRepository(client, apiKey, tokenProvider);
            });

            services.AddScoped<ITrackingEventRepository>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("supabase");
                var tokenProvider = sp.GetRequiredService<ISupabaseAccessTokenProvider>();
                var apiKey = configuration["Supabase:Key"]!;
                return new TrackingEventRepository(client, apiKey, tokenProvider);
            });

            return services;
        }

        public static IServiceCollection RegisterAuthServices(this IServiceCollection services)
        {
            services.AddSingleton<TestDataService>();
            services.AddSingleton<DeepLinkService>();
            services.AddScoped<SupabaseAuthService>();
            services.AddScoped<ISupabaseAccessTokenProvider>(sp => sp.GetRequiredService<SupabaseAuthService>());
            services.AddScoped<AuthStateService>();
            services.AddScoped<SupabaseStorageService>();
            return services;
        }

        public static IServiceCollection RegisterAppServices(this IServiceCollection services)
        {
            services.AddScoped<IAIDesignService, OpenAIDesignService>();
            services.AddHttpClient<OpenAIDesignService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            return services;
        }

        public static IServiceCollection RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                    .RegisterSupabase(configuration)
                    .RegisterRepositories(configuration)
                    .RegisterAuthServices()
                    .RegisterAppServices();
        }
    }
}