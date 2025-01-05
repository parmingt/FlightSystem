using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class Bookingprice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PriceId",
                table: "Bookings",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Currency",
                columns: ["Id",  "Name"],
                values: [Guid.NewGuid(), "USD"]);

            migrationBuilder.InsertData(
                table: "Currency",
                columns: ["Id", "Name"],
                values: [Guid.NewGuid(), "EUR"]);

            migrationBuilder.CreateTable(
                name: "Price",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Price", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Price_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PriceId",
                table: "Bookings",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_Price_CurrencyId",
                table: "Price",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Price_PriceId",
                table: "Bookings",
                column: "PriceId",
                principalTable: "Price",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Price_PriceId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "Price");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_PriceId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PriceId",
                table: "Bookings");
        }
    }
}
