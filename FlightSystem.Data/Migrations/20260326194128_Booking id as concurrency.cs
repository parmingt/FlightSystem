using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlightSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class Bookingidasconcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Segments_SegmentId",
                table: "Seats");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("277bdded-76ed-44d0-a94d-09270100cc7a"));

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("a0b86669-5553-4f61-9c90-4c91681304b0"));

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Seats");

            migrationBuilder.AlterColumn<Guid>(
                name: "SegmentId",
                table: "Seats",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("47f985ac-cce0-4b76-aa31-2a5f7717d7df"), "EUR" },
                    { new Guid("a19cde83-9094-4098-b357-1c7c841cd75e"), "USD" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Segments_SegmentId",
                table: "Seats",
                column: "SegmentId",
                principalTable: "Segments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Segments_SegmentId",
                table: "Seats");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("47f985ac-cce0-4b76-aa31-2a5f7717d7df"));

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("a19cde83-9094-4098-b357-1c7c841cd75e"));

            migrationBuilder.AlterColumn<Guid>(
                name: "SegmentId",
                table: "Seats",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Seats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("277bdded-76ed-44d0-a94d-09270100cc7a"), "EUR" },
                    { new Guid("a0b86669-5553-4f61-9c90-4c91681304b0"), "USD" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Segments_SegmentId",
                table: "Seats",
                column: "SegmentId",
                principalTable: "Segments",
                principalColumn: "Id");
        }
    }
}
