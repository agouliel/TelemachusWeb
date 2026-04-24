using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class PrevReportId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrevReportId",
                table: "reports",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_reports_PrevReportId",
                table: "reports",
                column: "PrevReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_reports_reports_PrevReportId",
                table: "reports",
                column: "PrevReportId",
                principalTable: "reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reports_reports_PrevReportId",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "IX_reports_PrevReportId",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "PrevReportId",
                table: "reports");
        }
    }
}
