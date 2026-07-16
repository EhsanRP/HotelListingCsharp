using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelListing.Migrations
{
    /// <inheritdoc />
    public partial class AddedRoleSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "09b68774-a7c2-485b-bdda-70228b89ab91",
                column: "ConcurrencyStamp",
                value: "829af75a-b871-47e8-b87a-4c2cf939c649");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bf768284-b943-4612-8e42-83097e51f6de",
                column: "ConcurrencyStamp",
                value: "937de25d-ca6a-4f52-acb8-594f57e5a8c3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "09b68774-a7c2-485b-bdda-70228b89ab91",
                column: "ConcurrencyStamp",
                value: "221ad347-9e96-4229-ac42-911039d5804f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bf768284-b943-4612-8e42-83097e51f6de",
                column: "ConcurrencyStamp",
                value: "fc24dac4-0d2a-4b19-8afc-0a4934cdcccd");
        }
    }
}
