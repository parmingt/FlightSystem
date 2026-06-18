using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlightSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addcodetosearchvector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("73dc9844-a4a5-474a-b40d-0b12f59d3750"));

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("bb83f166-c79c-4b61-b8cb-9312d56db87d"));

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "Airports",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Name", "Code" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "Name" });

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("0117a4c7-3c4e-429d-a1ec-9236977bc3e0"), "EUR" },
                    { new Guid("a9932ae4-7f36-449c-9e2f-8ac6ccdc4fa1"), "USD" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("0117a4c7-3c4e-429d-a1ec-9236977bc3e0"));

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("a9932ae4-7f36-449c-9e2f-8ac6ccdc4fa1"));

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "Airports",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "Name", "Code" });

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("73dc9844-a4a5-474a-b40d-0b12f59d3750"), "USD" },
                    { new Guid("bb83f166-c79c-4b61-b8cb-9312d56db87d"), "EUR" }
                });
        }
    }
}
