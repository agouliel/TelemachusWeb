using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class AddSettServStor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
               name: "SettlingService",
               table: "tanks",
               newName: "Settling");

            migrationBuilder.AddColumn<bool>(
                name: "Serving",
                table: "tanks",
                nullable: false,
                defaultValueSql: "CONVERT([bit],(0))");

            migrationBuilder.AddColumn<bool>(
                name: "Storage",
                table: "tanks",
                nullable: false,
                defaultValueSql: "CONVERT([bit],(1))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Serving",
                table: "tanks");

            migrationBuilder.DropColumn(
                name: "Storage",
                table: "tanks");

            migrationBuilder.RenameColumn(
                name: "Settling",
                table: "tanks",
                newName: "SettlingService");
        }
    }
}
