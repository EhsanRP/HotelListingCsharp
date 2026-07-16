using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Data;

public class HotelListingDbContext(DbContextOptions<HotelListingDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApiKey>(builder =>
        {
            builder.HasIndex(k => k.Key).IsUnique();
        });
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}