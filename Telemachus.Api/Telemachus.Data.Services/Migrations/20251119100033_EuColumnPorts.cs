using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class EuColumnPorts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsEuInt",
                table: "Ports",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEuInt",
                table: "Ports");
        }
    }
}
