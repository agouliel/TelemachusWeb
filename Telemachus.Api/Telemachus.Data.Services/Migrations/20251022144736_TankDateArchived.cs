using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class TankDateArchived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchived",
                table: "tank_user_specs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateArchived",
                table: "tank_user_specs");
        }
    }
}
