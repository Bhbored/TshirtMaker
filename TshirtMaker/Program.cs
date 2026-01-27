using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Supabase.Gotrue;
using TshirtMaker.Components;
using TshirtMaker.Services;

namespace TshirtMaker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure port from environment (Railway uses PORT env var)
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

            // Add logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // Configure forwarded headers FIRST
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                          ForwardedHeaders.XForwardedProto |
                                          ForwardedHeaders.XForwardedHost;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.ForwardLimit = null;
            });

            // Data Protection with ephemeral keys (since container restarts)
            builder.Services.AddDataProtection()
                .SetApplicationName("TshirtMaker");
            // Note: Keys won't persist between deployments, but that's OK for antiforgery

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddHubOptions(o =>
                {
                    o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
                    o.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                    o.HandshakeTimeout = TimeSpan.FromSeconds(60);
                    o.KeepAliveInterval = TimeSpan.FromSeconds(15);
                });

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromHours(24);
                options.Cookie.Name = ".TshirtMaker.Session";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.None;
            });

            builder.Services.AddHttpContextAccessor();

            var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL")
                ?? throw new InvalidOperationException("SUPABASE_URL environment variable is missing.");
            var supabaseAnonKey = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY")
                ?? throw new InvalidOperationException("SUPABASE_ANON_KEY environment variable is missing.");
            var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            builder.Services.RegisterDependencies(supabaseUrl, supabaseAnonKey, openAiApiKey);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
            });

            var app = builder.Build();

            // UseForwardedHeaders MUST be first
            app.UseForwardedHeaders();

            // Diagnostic logging
            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation($"Request: {context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}");
                logger.LogInformation($"X-Forwarded-Proto: {context.Request.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? "none"}");
                logger.LogInformation($"X-Forwarded-Host: {context.Request.Headers["X-Forwarded-Host"].FirstOrDefault() ?? "none"}");
                await next();
            });

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

            // Don't redirect HTTPS - Railway handles SSL
            // app.UseHttpsRedirection(); // <-- REMOVED

            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAntiforgery();
            app.MapStaticAssets();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}