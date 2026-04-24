using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class OnEventDeleteSetNullCargoDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_events_CargoDetailId",
                table: "events");

            migrationBuilder.CreateIndex(
                name: "IX_events_CargoDetailId",
                table: "events",
                column: "CargoDetailId",
                unique: true,
                filter: "[CargoDetailId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_events_CargoDetailId",
                table: "events");

            migrationBuilder.CreateIndex(
                name: "IX_events_CargoDetailId",
                table: "events",
                column: "CargoDetailId");
        }
    }
}
