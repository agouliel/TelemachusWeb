using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class ReportContextPrevContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrevContextId",
                table: "ReportContext",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportContext_PrevContextId",
                table: "ReportContext",
                column: "PrevContextId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportContext_ReportContext_PrevContextId",
                table: "ReportContext",
                column: "PrevContextId",
                principalTable: "ReportContext",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportContext_ReportContext_PrevContextId",
                table: "ReportContext");

            migrationBuilder.DropIndex(
                name: "IX_ReportContext_PrevContextId",
                table: "ReportContext");

            migrationBuilder.DropColumn(
                name: "PrevContextId",
                table: "ReportContext");
        }
    }
}
