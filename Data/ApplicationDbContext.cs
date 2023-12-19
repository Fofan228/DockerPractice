using Microsoft.EntityFrameworkCore;

namespace Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() : base()
    {
        var str = Database.GetConnectionString();
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        const string connectionString = "Host=postgres;Port=5432;Database=Links;Username=postgres;Password=root;CommandTimeout=300";
        
        optionsBuilder.UseNpgsql(connectionString);
    }

    public DbSet<Link> Links { get; set; }
}