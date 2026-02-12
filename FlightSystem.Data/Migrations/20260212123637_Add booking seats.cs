using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlightSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addbookingseats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("5a61b1c7-8f3f-4e8a-ab4f-b9f17cd85acc"));

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("d0021ea0-313a-4e18-8c21-1ec2a26ecc77"));

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("277bdded-76ed-44d0-a94d-09270100cc7a"), "EUR" },
                    { new Guid("a0b86669-5553-4f61-9c90-4c91681304b0"), "USD" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("277bdded-76ed-44d0-a94d-09270100cc7a"));

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("a0b86669-5553-4f61-9c90-4c91681304b0"));

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("5a61b1c7-8f3f-4e8a-ab4f-b9f17cd85acc"), "EUR" },
                    { new Guid("d0021ea0-313a-4e18-8c21-1ec2a26ecc77"), "USD" }
                });
        }
    }
}
