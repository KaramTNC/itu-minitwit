using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Web;

public class Program
{
    
    /// <summary>
    /// Main Program to run
    /// </summary>
    /// <param name="args">Optional arguments</param>
    public static void Main(string[] args)
    {
        var app = BuildWebApplication(args);
        
        //Initialise Database
        if (!app.Environment.IsEnvironment("Testing"))
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        
            try
            {
                if (context.Database.IsSqlite())
                {
                    context.Database.OpenConnection();
                }
            }
            catch (InvalidOperationException) { }
        
            context.Database.EnsureCreated();
            DbInitializer.SeedDatabase(context);
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
    /// <exception cref="Exception">Cannot get GitHub credentials</exception>
    public static WebApplication BuildWebApplication(string[]? args = null, string? environment = null)
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
                webProjectPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "src", "Web"));
                Console.WriteLine($"[Testing] Using fallback path: {webProjectPath}");
            }

            if (!Directory.Exists(webProjectPath))
            {
                throw new DirectoryNotFoundException($"Web project directory not found at: {webProjectPath}");
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

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            Args = args ?? Array.Empty<string>(),
            EnvironmentName = environment ?? Environments.Development,
            ContentRootPath = webProjectPath,
            WebRootPath = Path.Combine(webProjectPath, "wwwroot")
        });

        builder.Services.AddSession();
        builder.Services.AddDistributedMemoryCache();

        // Configure database based on environment
        if (builder.Environment.IsEnvironment("Testing"))
        {
            builder.Services.AddDbContext<ChatDbContext>(options =>
                options.UseSqlite("DataSource=TestDb;Mode=Memory;Cache=Shared"));
        }
        else
        {
            string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ChatDbContext>(options =>
                options.UseSqlite(connectionString));
        }

        // CRITICAL FIX: Use AddIdentity instead of AddDefaultIdentity
        // AddDefaultIdentity includes AddDefaultUI() which forces the RCL and prevents scaffolded pages from working
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
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
        .AddDefaultTokenProviders();  // Required for email confirmation, password reset, etc.

        builder.Services.AddTransient<IEmailSender, NoOpEmailSender>();
        
        // Load User Secrets
        if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
        {
            builder.Configuration.AddUserSecrets<Program>(optional: true);
        }

        var githubClientId = builder.Configuration["authentication_github_clientId"];
        var githubClientSecret = builder.Configuration["authentication_github_clientSecret"];
        var hasGitHubCredentials = !string.IsNullOrEmpty(githubClientId) && !string.IsNullOrEmpty(githubClientSecret);

        // Ensure github credentials exist
        if (!builder.Environment.IsDevelopment() && !builder.Environment.IsEnvironment("Testing") &&
            !hasGitHubCredentials)
        {
            throw new Exception("GitHub OAuth credentials missing");
        }

        var authBuilder = builder.Services.AddAuthentication().AddCookie();
        if (hasGitHubCredentials)
        {
            Console.WriteLine($"[DEBUG] GitHub OAuth enabled - ClientId: {githubClientId}");
            authBuilder.AddGitHub(o =>
            {
                o.ClientId = githubClientId!;
                o.ClientSecret = githubClientSecret!;
                o.CallbackPath = new PathString("/git-login");

                o.Scope.Add("user:email");
                o.Scope.Add("read:user");
            });
        }
        else
        {
            Console.WriteLine("[DEBUG] GitHub OAuth disabled - no credentials configured");
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
        builder.Services.AddMvc()
            .AddApplicationPart(typeof(Program).Assembly);

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
        app.MapRazorPages();

        return app;
    }
}