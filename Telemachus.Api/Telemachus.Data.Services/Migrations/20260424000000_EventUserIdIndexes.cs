using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class EventUserIdIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_events_UserId_Timestamp",
                table: "events",
                columns: new[] { "UserId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_events_UserId_CurrentVoyageConditionKey",
                table: "events",
                columns: new[] { "UserId", "CurrentVoyageConditionKey" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_events_UserId_Timestamp",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_UserId_CurrentVoyageConditionKey",
                table: "events");
        }
    }
}
