using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Data.Configurations;

public class RoleConfigurations : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole {Id="bf768284-b943-4612-8e42-83097e51f6de",ConcurrencyStamp = "937de25d-ca6a-4f52-acb8-594f57e5a8c3", Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
            new IdentityRole {Id="09b68774-a7c2-485b-bdda-70228b89ab91",ConcurrencyStamp = "829af75a-b871-47e8-b87a-4c2cf939c649",Name = "User", NormalizedName = "USER" },
            new IdentityRole {Id="6561611b-3696-4749-bbe5-5f347224b665",ConcurrencyStamp = "d523c0f3-7a3e-469f-a94a-f623a579fc85",Name = "Hotel Admin", NormalizedName = "HOTEL ADMIN" }
        );
    }
}