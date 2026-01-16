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

        public static IServiceCollection RegisterDependencies(this IServiceCollection services)
        {
            return services
                    .RegisterAuthServices();


        }
    }
}
