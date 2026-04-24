using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class StsOperations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sts_operations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    EventId = table.Column<int>(nullable: false),
                    ReverseLightering = table.Column<bool>(nullable: false),
                    CompanyParticipatingVesselId = table.Column<string>(nullable: true),
                    ParticipatingVessel = table.Column<string>(nullable: true),
                    SameSizeParticipatingVessel = table.Column<bool>(nullable: false),
                    SeaState = table.Column<short>(nullable: true),
                    BusinessId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sts_operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sts_operations_AspNetUsers_CompanyParticipatingVesselId",
                        column: x => x.CompanyParticipatingVesselId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_sts_operations_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_sts_operations_BusinessId",
                table: "sts_operations",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sts_operations_CompanyParticipatingVesselId",
                table: "sts_operations",
                column: "CompanyParticipatingVesselId");

            migrationBuilder.CreateIndex(
                name: "IX_sts_operations_DateModified",
                table: "sts_operations",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_sts_operations_EventId",
                table: "sts_operations",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sts_operations_IsDeleted",
                table: "sts_operations",
                column: "IsDeleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sts_operations");
        }
    }
}
