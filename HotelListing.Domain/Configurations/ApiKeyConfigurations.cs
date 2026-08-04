using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Domain.Configurations;

public class ApiKeyConfigurations : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.HasIndex(a => a.AppName).IsUnique();
        builder.HasData(
            new ApiKey
            {
                Id = 1,
                AppName = "AppName",
                CreatedAtUtc = new DateTime(2026, 01, 01),
                Key = "OWZkOGE1MWUtMDgzMy00ZWYzLTlhNmYtY2I0ZmZhZGQ5OTYz"
            }
        );
    }
}