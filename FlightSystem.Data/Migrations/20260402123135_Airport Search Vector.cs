using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlightSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AirportSearchVector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("47f985ac-cce0-4b76-aa31-2a5f7717d7df"));

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("a19cde83-9094-4098-b357-1c7c841cd75e"));

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "Airports",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Name" });

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("73dc9844-a4a5-474a-b40d-0b12f59d3750"), "USD" },
                    { new Guid("bb83f166-c79c-4b61-b8cb-9312d56db87d"), "EUR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Airports_SearchVector",
                table: "Airports",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Airports_SearchVector",
                table: "Airports");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("73dc9844-a4a5-474a-b40d-0b12f59d3750"));

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("bb83f166-c79c-4b61-b8cb-9312d56db87d"));

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "Airports");

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("47f985ac-cce0-4b76-aa31-2a5f7717d7df"), "EUR" },
                    { new Guid("a19cde83-9094-4098-b357-1c7c841cd75e"), "USD" }
                });
        }
    }
}
