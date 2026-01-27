using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using TshirtMaker.Components;
using TshirtMaker.Services;

namespace TshirtMaker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
                options.ForwardLimit = null;

                options.RequireHeaderSymmetry = false;
                options.ForwardedProtoHeaderName = "X-Forwarded-Proto";
            });

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

            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.IdleTimeout = TimeSpan.FromHours(24);
                options.Cookie.Name = ".TshirtMaker.Session";
            });

            builder.Services.AddAntiforgery(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
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

            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("=== Incoming Headers ===");
                foreach (var header in context.Request.Headers)
                {
                    logger.LogInformation($"{header.Key}: {header.Value}");
                }
                logger.LogInformation($"Scheme: {context.Request.Scheme}");
                logger.LogInformation("=======================");
                await next();
            });
            app.UseForwardedHeaders();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }


            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();
            app.UseSession();
            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}