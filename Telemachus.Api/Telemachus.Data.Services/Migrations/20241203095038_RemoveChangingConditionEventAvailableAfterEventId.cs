using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class RemoveChangingConditionEventAvailableAfterEventId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_events_events_ChangingConditionEventAvailableAfterEventId",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_ChangingConditionEventAvailableAfterEventId",
                table: "events");

            migrationBuilder.DropColumn(
                name: "ChangingConditionEventAvailableAfterEventId",
                table: "events");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChangingConditionEventAvailableAfterEventId",
                table: "events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_events_ChangingConditionEventAvailableAfterEventId",
                table: "events",
                column: "ChangingConditionEventAvailableAfterEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_events_events_ChangingConditionEventAvailableAfterEventId",
                table: "events",
                column: "ChangingConditionEventAvailableAfterEventId",
                principalTable: "events",
                principalColumn: "Id");
        }
    }
}
