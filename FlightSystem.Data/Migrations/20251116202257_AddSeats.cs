using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSeats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Segments_Bookings_BookingId",
                table: "Segments");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "Segments",
                newName: "FlightId");

            migrationBuilder.RenameIndex(
                name: "IX_Segments_BookingId",
                table: "Segments",
                newName: "IX_Segments_FlightId");

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BookingId = table.Column<Guid>(type: "TEXT", nullable: true),
                    FlightId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seats_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Seats_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Seats_BookingId",
                table: "Seats",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_FlightId",
                table: "Seats",
                column: "FlightId");

            migrationBuilder.AddForeignKey(
                name: "FK_Segments_Flights_FlightId",
                table: "Segments",
                column: "FlightId",
                principalTable: "Flights",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Segments_Flights_FlightId",
                table: "Segments");

            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.RenameColumn(
                name: "FlightId",
                table: "Segments",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Segments_FlightId",
                table: "Segments",
                newName: "IX_Segments_BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Segments_Bookings_BookingId",
                table: "Segments",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");
        }
    }
}
