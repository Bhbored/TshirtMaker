using TshirtMaker.Services.AI;

namespace TshirtMaker.Services
{
    public static class DIContainer
    {

        public static IServiceCollection RegisterAuthServices(this IServiceCollection services)
        {
            services.AddSingleton<TestDataService>();
            services.AddSingleton<AuthService>();
            services.AddSingleton<DeepLinkService>();
            return services;
        }

        public static IServiceCollection RegisterAppServices(this IServiceCollection services)
        {
            services.AddScoped<IAIDesignService, GeminiAIBananaService>();
            services.AddHttpClient(); 
            return services;
        }

        public static IServiceCollection RegisterDependencies(this IServiceCollection services)
        {
            return services
                    .RegisterAuthServices()
                    .RegisterAppServices();


        }
    }
}
