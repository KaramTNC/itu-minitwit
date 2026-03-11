using Core.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

/// <summary>
/// Creates our entity framework for our database
/// </summary>
public class ChatDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// These DbSets represents the collection of all entities in the context,
    /// or that can be queried from the database, of a given type.
    /// DbSet objects are created from a DbContext using the DbContext.Set method.
    /// </summary>
    public DbSet<Cheep> Cheeps { get; set; }

    public DbSet<Author> Authors { get; set; }

    public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options) { }

    public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Cheep>(entity =>
        {
            entity.ToTable("cheeps");
            entity.HasKey(e => e.CheepId);
            entity.Property(e => e.CheepId).HasColumnName("cheepid");
            entity.Property(e => e.AuthorId).HasColumnName("authorid");
            entity.Property(e => e.Text).HasColumnName("text");
            entity.Property(e => e.TimeStamp).HasColumnName("timestamp");
            entity.Property(e => e.PeopleLikes).HasColumnName("peoplelikes");
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("authors");
            entity.HasKey(e => e.AuthorId);
            entity.Property(e => e.AuthorId).HasColumnName("authorid");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Follows).HasColumnName("follows");
            entity.Property(e => e.CheepLikes).HasColumnName("cheeplikes");
        });

        modelBuilder.Entity<IdentityUser>().ToTable("aspnetusers");
        modelBuilder.Entity<IdentityRole>().ToTable("aspnetroles");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("aspnetuserroles");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("aspnetuserclaims");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("aspnetuserlogins");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("aspnetusertokens");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("aspnetroleclaims");
    }
}
