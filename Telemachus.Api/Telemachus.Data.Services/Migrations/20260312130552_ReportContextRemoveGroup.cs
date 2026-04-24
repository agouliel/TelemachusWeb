using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class ReportContextRemoveGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportContext_report_field_groups_GroupId",
                table: "ReportContext");

            migrationBuilder.DropIndex(
                name: "IX_ReportContext_GroupId",
                table: "ReportContext");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "ReportContext");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "ReportContext",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ReportContext_GroupId",
                table: "ReportContext",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportContext_report_field_groups_GroupId",
                table: "ReportContext",
                column: "GroupId",
                principalTable: "report_field_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
