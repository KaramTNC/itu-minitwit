using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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

    

    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
    {
        
    }
    
    /*
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }*/
}