using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Domain;

public class HotelListingDbContext(DbContextOptions<HotelListingDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<HotelAdmin> HotelAdmins { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApiKey>(builder => { builder.HasIndex(k => k.Key).IsUnique(); });
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Country>()
            .HasIndex(c => c.Name)
            .HasDatabaseName("IX_Countries_Name");
        modelBuilder.Entity<Country>()
            .HasIndex(c => c.ShortName)
            .HasDatabaseName("IX_Countries_ShortName");

        modelBuilder.Entity<Hotel>()
            .HasIndex(h => h.Name)
            .HasDatabaseName("IX_Hotels_Name");
        modelBuilder.Entity<Hotel>()
            .HasIndex(h => h.CountryId)
            .HasDatabaseName("IX_Hotels_CountryId");
        modelBuilder.Entity<Hotel>()
            .HasIndex(h => new { h.CountryId, h.Rating })
            .HasDatabaseName("IX_Hotels_CountryId_Rating");
    }
}