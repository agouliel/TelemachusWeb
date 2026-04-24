using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class TankOverflow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Overflow",
                table: "tanks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Overflow",
                table: "tanks");
        }
    }
}
