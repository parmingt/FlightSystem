using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlightSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class Makebookingidnullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("6267fe93-1d65-415c-a28a-8bee1a70b765"));

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("b8bdc1c7-5204-4075-92d6-46f980d6a16e"));

            migrationBuilder.AlterColumn<string>(
                name: "BookingId",
                table: "Bookings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("5a61b1c7-8f3f-4e8a-ab4f-b9f17cd85acc"), "EUR" },
                    { new Guid("d0021ea0-313a-4e18-8c21-1ec2a26ecc77"), "USD" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("5a61b1c7-8f3f-4e8a-ab4f-b9f17cd85acc"));

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("d0021ea0-313a-4e18-8c21-1ec2a26ecc77"));

            migrationBuilder.AlterColumn<string>(
                name: "BookingId",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("6267fe93-1d65-415c-a28a-8bee1a70b765"), "EUR" },
                    { new Guid("b8bdc1c7-5204-4075-92d6-46f980d6a16e"), "USD" }
                });
        }
    }
}
