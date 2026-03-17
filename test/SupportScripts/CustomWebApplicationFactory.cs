using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web;

namespace SupportScripts;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // remove the existing context configuration
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<ChatDbContext>)
            );
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<ChatDbContext>(options => options.UseInMemoryDatabase("TestDB"));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        context.Database.EnsureCreated();
        DbInitializer.SeedDatabase(context);
        return host;
    }
}
