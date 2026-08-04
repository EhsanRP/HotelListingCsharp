using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Domain.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(p => p.StatusEnum)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(p => new { p.HotelId, p.UserId });
        /*
        builder.HasIndex(p => p.HotelId);
        builder.HasIndex(p => p.UserId);
        Code Above Merges These Two
        */
    }
}