using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Data.Configurations;

public class RoleConfigurations : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole {Id="bf768284-b943-4612-8e42-83097e51f6de", Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
            new IdentityRole {Id="09b68774-a7c2-485b-bdda-70228b89ab91", Name = "User", NormalizedName = "USER" }
        );
    }
}