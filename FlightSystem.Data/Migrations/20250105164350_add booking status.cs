using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class addbookingstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Bookings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BookingStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StatusId",
                table: "Bookings",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_BookingStatus_StatusId",
                table: "Bookings",
                column: "StatusId",
                principalTable: "BookingStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_BookingStatus_StatusId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "BookingStatus");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_StatusId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Bookings");
        }
    }
}
