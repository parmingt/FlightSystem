using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixphiladelphia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: new Guid("a22f127a-05bb-4c62-9cdd-98b6470a536b"),
                column: "Code",
                value: "PHL");

            migrationBuilder.UpdateData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: new Guid("f6ca3782-ecfd-4417-b588-b1d988faf97b"),
                column: "Departure",
                value: new DateTime(2024, 10, 3, 8, 47, 28, 838, DateTimeKind.Local).AddTicks(5487));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: new Guid("a22f127a-05bb-4c62-9cdd-98b6470a536b"),
                column: "Code",
                value: "PHI");

            migrationBuilder.UpdateData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: new Guid("f6ca3782-ecfd-4417-b588-b1d988faf97b"),
                column: "Departure",
                value: new DateTime(2024, 10, 1, 13, 7, 18, 418, DateTimeKind.Local).AddTicks(2861));
        }
    }
}
