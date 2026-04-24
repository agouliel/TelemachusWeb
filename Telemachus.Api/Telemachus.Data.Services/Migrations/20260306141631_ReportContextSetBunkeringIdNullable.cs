using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class ReportContextSetBunkeringIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportContext_bunkering_data_BunkeringId",
                table: "ReportContext");

            migrationBuilder.DropIndex(
                name: "IX_ReportContext_ReportId_TankId_GroupId",
                table: "ReportContext");

            migrationBuilder.AlterColumn<int>(
                name: "BunkeringId",
                table: "ReportContext",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ReportContext_ReportId",
                table: "ReportContext",
                column: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportContext_bunkering_data_BunkeringId",
                table: "ReportContext",
                column: "BunkeringId",
                principalTable: "bunkering_data",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportContext_bunkering_data_BunkeringId",
                table: "ReportContext");

            migrationBuilder.DropIndex(
                name: "IX_ReportContext_ReportId",
                table: "ReportContext");

            migrationBuilder.AlterColumn<int>(
                name: "BunkeringId",
                table: "ReportContext",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportContext_ReportId_TankId_GroupId",
                table: "ReportContext",
                columns: new[] { "ReportId", "TankId", "GroupId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportContext_bunkering_data_BunkeringId",
                table: "ReportContext",
                column: "BunkeringId",
                principalTable: "bunkering_data",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
