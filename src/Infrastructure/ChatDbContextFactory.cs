using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure;

public class ChatDbContextFactory : IDesignTimeDbContextFactory<ChatDbContext>
{
    public ChatDbContext CreateDbContext(string[] args)
    {
        // Walk up from Infrastructure/ to solution root to find .env
        DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env"));

        var dbUrl = Environment.GetEnvironmentVariable("DB_URL")
                    ?? throw new InvalidOperationException("DB_URL not set in .env");

        var optionsBuilder = new DbContextOptionsBuilder<ChatDbContext>();
        optionsBuilder.UseNpgsql(dbUrl);

        return new ChatDbContext(optionsBuilder.Options);
    }
}
