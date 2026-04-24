using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class FuelTypeTankRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FuelTypeId",
                table: "tanks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tanks_FuelTypeId",
                table: "tanks",
                column: "FuelTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_tanks_fuel_types_FuelTypeId",
                table: "tanks",
                column: "FuelTypeId",
                principalTable: "fuel_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tanks_fuel_types_FuelTypeId",
                table: "tanks");

            migrationBuilder.DropIndex(
                name: "IX_tanks_FuelTypeId",
                table: "tanks");

            migrationBuilder.DropColumn(
                name: "FuelTypeId",
                table: "tanks");
        }
    }
}
