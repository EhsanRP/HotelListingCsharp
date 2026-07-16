using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelListing.Migrations
{
    /// <inheritdoc />
    public partial class AddedDefaultApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ApiKeys",
                columns: new[] { "Id", "AppName", "CreatedAtUtc", "ExpiresAtUts", "Key" },
                values: new object[] { 1, "AppName", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 30, 0, 0)), null, "OWZkOGE1MWUtMDgzMy00ZWYzLTlhNmYtY2I0ZmZhZGQ5OTYz" });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_AppName",
                table: "ApiKeys",
                column: "AppName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApiKeys_AppName",
                table: "ApiKeys");

            migrationBuilder.DeleteData(
                table: "ApiKeys",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
