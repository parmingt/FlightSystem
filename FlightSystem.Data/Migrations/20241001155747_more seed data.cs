using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlightSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class moreseeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: new Guid("32be2570-3881-4be5-a074-9b295fd599e3"));

            migrationBuilder.InsertData(
                table: "Airports",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { new Guid("5654f336-9fab-4c97-be51-b6a14a51dcda"), "EWR", "Newark" },
                    { new Guid("a22f127a-05bb-4c62-9cdd-98b6470a536b"), "PHI", "Philadelphia" },
                    { new Guid("c4ac12df-ae7b-4b9e-99dc-79d3ec0fb6cf"), "SLC", "Salt Lake City" },
                    { new Guid("c71f5d8d-0cb5-4543-a311-1634e9e7bfc6"), "ATL", "Atlanta" }
                });

            migrationBuilder.InsertData(
                table: "Flights",
                columns: new[] { "Id", "Departure", "DestinationId", "Duration", "OriginId" },
                values: new object[] { new Guid("f6ca3782-ecfd-4417-b588-b1d988faf97b"), new DateTime(2024, 10, 1, 11, 57, 47, 634, DateTimeKind.Local).AddTicks(7434), new Guid("c71f5d8d-0cb5-4543-a311-1634e9e7bfc6"), new TimeSpan(0, 3, 0, 0, 0), new Guid("5654f336-9fab-4c97-be51-b6a14a51dcda") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: new Guid("a22f127a-05bb-4c62-9cdd-98b6470a536b"));

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: new Guid("c4ac12df-ae7b-4b9e-99dc-79d3ec0fb6cf"));

            migrationBuilder.DeleteData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: new Guid("f6ca3782-ecfd-4417-b588-b1d988faf97b"));

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: new Guid("5654f336-9fab-4c97-be51-b6a14a51dcda"));

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: new Guid("c71f5d8d-0cb5-4543-a311-1634e9e7bfc6"));

            migrationBuilder.InsertData(
                table: "Airports",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { new Guid("32be2570-3881-4be5-a074-9b295fd599e3"), "ATL", "Atlanta" });
        }
    }
}
