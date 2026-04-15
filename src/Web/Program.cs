using Core.Interfaces;
using DotNetEnv;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace Web;

public class Program
{
    /// <summary>
    /// Main Program to run
    /// </summary>
    /// <param name="args">Optional arguments</param>
    public static void Main(string[] args)
    {
        Env.Load("../../.env");
        var app = BuildWebApplication(args);
        var metricsUriPrefix =
            Environment.GetEnvironmentVariable("OTEL_PROMETHEUS_URI_PREFIX") ?? "http://*:9184/";

        // Necessary for monitoring metrics
        // Initialises a metrics endpoint where Prometheus can scrape (and store) the metrics gathered by OpenTelemetry.
        using MeterProvider meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddMeter("Web.Public") // This meter is currently the only one used. We'll need to add more meters using .AddMeter later on.
            .AddPrometheusHttpListener(options =>
                options.UriPrefixes = new string[] { metricsUriPrefix }
            ) // endpoint is http://<host>:9184/metrics
            .Build();

        //Initialise Database
        if (!app.Environment.IsEnvironment("Testing"))
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

            try
            {
                if (context.Database.IsNpgsql())
                {
                    context.Database.OpenConnection();
                }
            }
            catch (InvalidOperationException) { }

            if (!context.Database.CanConnect())
            {
                throw new InvalidOperationException("Database connection failed.");
            }
        }

        app.Run();
    }

    /// <summary>
    /// Build a WebApplication depending on the runtime environment
    /// </summary>
    /// <param name="args">Optional arguments</param>
    /// <param name="environment">Optional environment to specify</param>
    /// <returns>Webapplication</returns>
    /// <exception cref="DirectoryNotFoundException">Cannot find Web directory</exception>
    public static WebApplication BuildWebApplication(
        string[]? args = null,
        string? environment = null
    )
    {
        var baseDir = AppContext.BaseDirectory;
        string webProjectPath;

        // Determine the correct content root path
        if (environment == "Testing")
        {
            var currentDir = new DirectoryInfo(baseDir);
            DirectoryInfo? solutionRoot = null;

            while (currentDir != null)
            {
                if (Directory.GetFiles(currentDir.FullName, "*.sln").Length > 0)
                {
                    solutionRoot = currentDir;
                    break;
                }

                var webCsprojPath = Path.Combine(currentDir.FullName, "src", "Web", "Web.csproj");
                if (File.Exists(webCsprojPath))
                {
                    solutionRoot = currentDir;
                    break;
                }

                currentDir = currentDir.Parent;
            }

            if (solutionRoot != null)
            {
                webProjectPath = Path.Combine(solutionRoot.FullName, "src", "Web");
                Console.WriteLine($"[Testing] Found Web project at: {webProjectPath}");
            }
            else
            {
                webProjectPath = Path.GetFullPath(
                    Path.Combine(baseDir, "..", "..", "..", "..", "..", "src", "Web")
                );
                Console.WriteLine($"[Testing] Using fallback path: {webProjectPath}");
            }

            if (!Directory.Exists(webProjectPath))
            {
                throw new DirectoryNotFoundException(
                    $"Web project directory not found at: {webProjectPath}"
                );
            }

            var areasPath = Path.Combine(webProjectPath, "Areas");
            var pagesPath = Path.Combine(webProjectPath, "Pages");
            Console.WriteLine($"[Testing] Areas folder exists: {Directory.Exists(areasPath)}");
            Console.WriteLine($"[Testing] Pages folder exists: {Directory.Exists(pagesPath)}");
        }
        else
        {
            var currentDir = new DirectoryInfo(baseDir);
            DirectoryInfo? foundDir = null;

            while (currentDir != null)
            {
                var webCsprojDirect = Path.Combine(currentDir.FullName, "Web.csproj");
                var webCsprojInSrc = Path.Combine(currentDir.FullName, "src", "Web", "Web.csproj");

                if (File.Exists(webCsprojDirect))
                {
                    foundDir = currentDir;
                    break;
                }

                if (File.Exists(webCsprojInSrc))
                {
                    foundDir = new DirectoryInfo(Path.Combine(currentDir.FullName, "src", "Web"));
                    break;
                }

                currentDir = currentDir.Parent;
            }

            webProjectPath = foundDir?.FullName ?? baseDir;
        }

        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions()
            {
                Args = args ?? Array.Empty<string>(),
                EnvironmentName = environment ?? Environments.Development,
                ContentRootPath = webProjectPath,
                WebRootPath = Path.Combine(webProjectPath, "wwwroot"),
            }
        );

        builder.Services.AddSession();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database");

        var dataProtectionKeysPath = Environment.GetEnvironmentVariable(
            "ASPNETCORE_DATA_PROTECTION_KEYS"
        );
        if (!string.IsNullOrWhiteSpace(dataProtectionKeysPath))
        {
            builder
                .Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath));
        }

        // Configure database based on environment
        if (builder.Environment.IsEnvironment("Testing"))
        {
            builder.Services.AddDbContext<ChatDbContext>(options =>
                options.UseSqlite("DataSource=TestDb;Mode=Memory;Cache=Shared")
            );
        }
        else
        {
            string? envVarName = builder.Configuration.GetConnectionString("DefaultConnection");
            if (envVarName == null)
                throw new Exception("DefaultConnection key not found in appsettings");

            var rawConnectionString =
                Environment.GetEnvironmentVariable(envVarName)
                ?? builder.Configuration.GetConnectionString("DefaultConnection"); // fallback

            if (rawConnectionString == null)
                throw new Exception($"Environment variable '{envVarName}' is not set");

            var connectionString = NormalizeNpgsqlConnectionString(rawConnectionString);

            builder.Services.AddDbContext<ChatDbContext>(options =>
                options.UseNpgsql(connectionString)
            );
        }

        // CRITICAL FIX: Use AddIdentity instead of AddDefaultIdentity
        // AddDefaultIdentity includes AddDefaultUI() which forces the RCL and prevents scaffolded pages from working
        builder
            .Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                // Add any other identity options you need
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            })
            .AddEntityFrameworkStores<ChatDbContext>()
            .AddDefaultTokenProviders(); // Required for email confirmation, password reset, etc.

        builder.Services.AddTransient<IEmailSender, NoOpEmailSender>();

        // Load User Secrets
        if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
        {
            builder.Configuration.AddUserSecrets<Program>(optional: true);
        }

        // Configure Razor Pages with runtime compilation
        var razorPagesBuilder = builder.Services.AddRazorPages(options =>
        {
            // Configure Razor Pages to allow Areas (required for Identity)
            options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
            options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
        });

        // Enable runtime compilation for Testing and Development
        if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
        {
            razorPagesBuilder.AddRazorRuntimeCompilation();
        }

        // Explicitly configure MVC to use the Web assembly
        builder.Services.AddMvc().AddApplicationPart(typeof(Program).Assembly);

        builder.Services.AddScoped<ICheepService, CheepService>();
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();
        builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.SlidingExpiration = true;
        });

        var app = builder.Build();

        // Log important paths in Testing environment
        if (app.Environment.IsEnvironment("Testing"))
        {
            Console.WriteLine($"[Testing] ContentRootPath: {app.Environment.ContentRootPath}");
            Console.WriteLine($"[Testing] WebRootPath: {app.Environment.WebRootPath}");
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsEnvironment("Testing"))
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }
        }

        if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
        {
            app.UseHttpsRedirection();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        app.MapHealthChecks("/health");
        app.MapRazorPages();

        return app;
    }

    public static string NormalizeNpgsqlConnectionString(string connectionString)
    {
        if (
            Uri.TryCreate(connectionString, UriKind.Absolute, out var uri)
            && (uri.Scheme == "postgres" || uri.Scheme == "postgresql")
        )
        {
            return ToNpgsqlConnectionString(uri);
        }

        return connectionString;
    }

    static string ToNpgsqlConnectionString(Uri uri)
    {
        var userInfo = uri.UserInfo.Split(':', 2);
        var db = uri.AbsolutePath.TrimStart('/');
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var sslMode = query["sslmode"] ?? "Require";

        return $"Host={uri.Host};Port={uri.Port};Database={db};Username={Uri.UnescapeDataString(userInfo[0])};Password={Uri.UnescapeDataString(userInfo[1])};SSL Mode={sslMode};Trust Server Certificate=true;";
    }
}
