using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class EventTypePrerequisitesCols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_event_types_event_types_AvailableAfterEventTypeId",
            //    table: "event_types");

            //migrationBuilder.DropIndex(
            //    name: "IX_event_types_AvailableAfterEventTypeId",
            //    table: "event_types");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "EventTypePrerequisites",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Optional",
                table: "EventTypePrerequisites",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "EventTypePrerequisites");

            migrationBuilder.DropColumn(
                name: "Optional",
                table: "EventTypePrerequisites");

            //migrationBuilder.CreateIndex(
            //    name: "IX_event_types_AvailableAfterEventTypeId",
            //    table: "event_types",
            //    column: "AvailableAfterEventTypeId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_event_types_event_types_AvailableAfterEventTypeId",
            //    table: "event_types",
            //    column: "AvailableAfterEventTypeId",
            //    principalTable: "event_types",
            //    principalColumn: "Id");
        }
    }
}
