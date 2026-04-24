using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class ReportContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReportContextId",
                table: "report_field_values",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReportContext",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(nullable: false),
                    TankId = table.Column<int>(nullable: false),
                    BunkeringId = table.Column<int>(nullable: false),
                    GroupId = table.Column<int>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportContext", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportContext_bunkering_data_BunkeringId",
                        column: x => x.BunkeringId,
                        principalTable: "bunkering_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportContext_report_field_groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "report_field_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportContext_reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "reports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportContext_tanks_TankId",
                        column: x => x.TankId,
                        principalTable: "tanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_report_field_values_ReportContextId",
                table: "report_field_values",
                column: "ReportContextId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportContext_BunkeringId",
                table: "ReportContext",
                column: "BunkeringId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportContext_DateModified",
                table: "ReportContext",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_ReportContext_GroupId",
                table: "ReportContext",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportContext_TankId",
                table: "ReportContext",
                column: "TankId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportContext_ReportId_TankId_GroupId",
                table: "ReportContext",
                columns: new[] { "ReportId", "TankId", "GroupId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_report_field_values_ReportContext_ReportContextId",
                table: "report_field_values",
                column: "ReportContextId",
                principalTable: "ReportContext",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_report_field_values_ReportContext_ReportContextId",
                table: "report_field_values");

            migrationBuilder.DropTable(
                name: "ReportContext");

            migrationBuilder.DropIndex(
                name: "IX_report_field_values_ReportContextId",
                table: "report_field_values");

            migrationBuilder.DropColumn(
                name: "ReportContextId",
                table: "report_field_values");
        }
    }
}
