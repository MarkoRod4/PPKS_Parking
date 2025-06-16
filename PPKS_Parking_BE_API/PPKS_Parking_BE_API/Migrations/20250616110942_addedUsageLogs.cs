using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPKS_Parking_BE_API.Migrations
{
    /// <inheritdoc />
    public partial class addedUsageLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParkingSpotUsageLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParkingSpotId = table.Column<int>(type: "int", nullable: false),
                    IsOccupied = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingSpotUsageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParkingSpotUsageLogs_ParkingSpots_ParkingSpotId",
                        column: x => x.ParkingSpotId,
                        principalTable: "ParkingSpots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpotUsageLogs_ParkingSpotId",
                table: "ParkingSpotUsageLogs",
                column: "ParkingSpotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParkingSpotUsageLogs");
        }
    }
}
