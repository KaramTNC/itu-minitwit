using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure;

public class ChatDbContextFactory : IDesignTimeDbContextFactory<ChatDbContext>
{
    public ChatDbContext CreateDbContext(string[] args)
    {
        var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
        if (File.Exists(envPath))
            DotNetEnv.Env.Load(envPath);

        var connectionString = 
            Environment.GetEnvironmentVariable("DB_URL")
            ?? "Host=localhost;Database=design-time;Username=dummy;Password=dummy";

        var optionsBuilder = new DbContextOptionsBuilder<ChatDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new ChatDbContext(optionsBuilder.Options);
    }
}
