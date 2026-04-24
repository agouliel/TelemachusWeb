using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class EventTypePrerequisites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventTypePrerequisites",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    EventTypeId = table.Column<int>(nullable: false),
                    AvailableAfterEventTypeId = table.Column<int>(nullable: false),
                    BusinessId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypePrerequisites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTypePrerequisites_event_types_AvailableAfterEventTypeId",
                        column: x => x.AvailableAfterEventTypeId,
                        principalTable: "event_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventTypePrerequisites_event_types_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "event_types",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventTypePrerequisites_AvailableAfterEventTypeId",
                table: "EventTypePrerequisites",
                column: "AvailableAfterEventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTypePrerequisites_BusinessId",
                table: "EventTypePrerequisites",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventTypePrerequisites_DateModified",
                table: "EventTypePrerequisites",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_EventTypePrerequisites_IsDeleted",
                table: "EventTypePrerequisites",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EventTypePrerequisites_EventTypeId_AvailableAfterEventTypeId",
                table: "EventTypePrerequisites",
                columns: new[] { "EventTypeId", "AvailableAfterEventTypeId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventTypePrerequisites");
        }
    }
}
