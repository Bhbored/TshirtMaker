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

            // Configure forwarded headers FIRST - this is critical for proxy scenarios
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                          ForwardedHeaders.XForwardedProto |
                                          ForwardedHeaders.XForwardedHost;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.ForwardLimit = null;
            });

            // Configure Data Protection to persist keys and use a stable application name
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("/app/keys")) // Adjust path as needed
                .SetApplicationName("TshirtMaker");

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddHubOptions(o =>
                {
                    o.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10 MB
                    o.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                    o.HandshakeTimeout = TimeSpan.FromSeconds(60);
                    o.KeepAliveInterval = TimeSpan.FromSeconds(15);
                });

            builder.Services.AddDistributedMemoryCache();

            // Fix session cookie configuration
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromHours(24);
                options.Cookie.Name = ".TshirtMaker.Session";

                // Use environment-aware cookie settings
                if (builder.Environment.IsDevelopment())
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                }
                else
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.None;
                }
            });

            builder.Services.AddHttpContextAccessor();

            var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL")
                ?? throw new InvalidOperationException("SUPABASE_URL environment variable is missing.");
            var supabaseAnonKey = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY")
                ?? throw new InvalidOperationException("SUPABASE_ANON_KEY environment variable is missing.");
            var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            builder.Services.RegisterDependencies(supabaseUrl, supabaseAnonKey, openAiApiKey);

            if (!builder.Environment.IsDevelopment())
            {
                builder.WebHost.ConfigureKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
                });
            }

            var app = builder.Build();

            // UseForwardedHeaders MUST be called early, before other middleware
            app.UseForwardedHeaders();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

            // For Railway, you might want to remove or conditionally apply HTTPS redirection
            if (builder.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles(); // Add this if missing
            app.UseSession();
            app.UseAntiforgery();
            app.MapStaticAssets();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}