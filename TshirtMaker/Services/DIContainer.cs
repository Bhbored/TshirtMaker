using Supabase;
using TshirtMaker.Repositories;
using TshirtMaker.Repositories.Interfaces;
using TshirtMaker.Services.AI;
using TshirtMaker.Services.Supabase;

namespace TshirtMaker.Services
{
    public static class DIContainer
    {
        public static IServiceCollection RegisterSupabase(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase:Url not configured");
            var key = configuration["Supabase:Key"] ?? throw new InvalidOperationException("Supabase:Key not configured");

            services.AddSingleton(provider =>
            {
                var options = new SupabaseOptions
                {
                    AutoRefreshToken = true,
                    AutoConnectRealtime = false,
                    SessionHandler = new SupabaseSessionHandler()
                };

                var client = new global::Supabase.Client(url, key, options);
                client.InitializeAsync().Wait();
                return client;
            });

            return services;
        }

        public static IServiceCollection RegisterRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase:Url not configured");
            var key = configuration["Supabase:Key"] ?? throw new InvalidOperationException("Supabase:Key not configured");

            services.AddScoped<IUserRepository>(sp => new UserRepository(sp.GetRequiredService<global::Supabase.Client>(), url, key));
            services.AddScoped<IDesignRepository>(sp => new DesignRepository(sp.GetRequiredService<global::Supabase.Client>(), url, key));
            services.AddScoped<IPostRepository>(sp => new PostRepository(sp.GetRequiredService<global::Supabase.Client>(), url, key));
            services.AddScoped<ICommentRepository>(sp => new CommentRepository(sp.GetRequiredService<global::Supabase.Client>(), url, key));
            services.AddScoped<ILikeRepository>(sp => new LikeRepository(sp.GetRequiredService<global::Supabase.Client>(), url, key));
            services.AddScoped<IBookmarkRepository>(sp => new BookmarkRepository(sp.GetRequiredService<global::Supabase.Client>(), url, key));
            services.AddScoped<IFollowerRepository>(sp => new FollowerRepository(sp.GetRequiredService<global::Supabase.Client>(), url, key));
            services.AddScoped<INotificationRepository>(sp => new NotificationRepository(sp.GetRequiredService<global::Supabase.Client>(), url, key));
            services.AddScoped<ICollectionRepository>(sp => new CollectionRepository(sp.GetRequiredService<global::Supabase.Client>(), url, key));
            services.AddScoped<IOrderRepository>(sp => new OrderRepository(sp.GetRequiredService<global::Supabase.Client>(), url, key));
            services.AddScoped<IOrderItemRepository>(sp => new OrderItemRepository(sp.GetRequiredService<global::Supabase.Client>(), url, key));
            services.AddScoped<IShippingAddressRepository>(sp => new ShippingAddressRepository(sp.GetRequiredService<global::Supabase.Client>(), url, key));
            services.AddScoped<ITrackingEventRepository>(sp => new TrackingEventRepository(sp.GetRequiredService<global::Supabase.Client>(), url, key));
            return services;
        }

        public static IServiceCollection RegisterAuthServices(this IServiceCollection services)
        {
            services.AddSingleton<TestDataService>();
            services.AddSingleton<AuthService>();
            services.AddSingleton<DeepLinkService>();
            services.AddScoped<SupabaseAuthService>();
            return services;
        }

        public static IServiceCollection RegisterAppServices(this IServiceCollection services)
        {
            services.AddScoped<IAIDesignService, GeminiAIBananaService>();
            services.AddHttpClient();
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
