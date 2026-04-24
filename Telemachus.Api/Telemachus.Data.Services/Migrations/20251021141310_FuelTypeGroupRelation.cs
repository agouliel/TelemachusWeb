using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class FuelTypeGroupRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FuelTypeId",
                table: "report_field_groups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_report_field_groups_FuelTypeId",
                table: "report_field_groups",
                column: "FuelTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_report_field_groups_fuel_types_FuelTypeId",
                table: "report_field_groups",
                column: "FuelTypeId",
                principalTable: "fuel_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_report_field_groups_fuel_types_FuelTypeId",
                table: "report_field_groups");

            migrationBuilder.DropIndex(
                name: "IX_report_field_groups_FuelTypeId",
                table: "report_field_groups");

            migrationBuilder.DropColumn(
                name: "FuelTypeId",
                table: "report_field_groups");
        }
    }
}
