using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class addforeignkeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Price_PriceId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_PriceId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PriceId",
                table: "Bookings");

            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                table: "Price",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Price_BookingId",
                table: "Price",
                column: "BookingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Price_Bookings_BookingId",
                table: "Price",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Price_Bookings_BookingId",
                table: "Price");

            migrationBuilder.DropIndex(
                name: "IX_Price_BookingId",
                table: "Price");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Price");

            migrationBuilder.AddColumn<Guid>(
                name: "PriceId",
                table: "Bookings",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PriceId",
                table: "Bookings",
                column: "PriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Price_PriceId",
                table: "Bookings",
                column: "PriceId",
                principalTable: "Price",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
