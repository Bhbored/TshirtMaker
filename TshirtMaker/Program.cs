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

            // Forwarded headers (for proxy/HTTPS)
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
                options.ForwardLimit = null;
                options.RequireHeaderSymmetry = false;
            });

            // Blazor Server
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddHubOptions(o =>
                {
                    o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
                    o.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                    o.HandshakeTimeout = TimeSpan.FromSeconds(60);
                    o.KeepAliveInterval = TimeSpan.FromSeconds(15);
                });

            // Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.IdleTimeout = TimeSpan.FromHours(24);
                options.Cookie.Name = ".TshirtMaker.Session";
            });

            builder.Services.AddHttpContextAccessor();

            // Your services
            var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL")
                ?? throw new InvalidOperationException("SUPABASE_URL missing.");
            var supabaseAnonKey = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY")
                ?? throw new InvalidOperationException("SUPABASE_ANON_KEY missing.");
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

            // Middleware order
            app.UseForwardedHeaders();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();
            app.UseSession();

            app.MapStaticAssets();
            app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

            app.Run();

        }
    }
}