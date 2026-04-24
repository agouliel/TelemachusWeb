using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class StsOperations2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeaState",
                table: "sts_operations");

            migrationBuilder.AlterColumn<string>(
                name: "ParticipatingVessel",
                table: "sts_operations",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "sts_operations",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RoughSeaState",
                table: "sts_operations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "sts_operations");

            migrationBuilder.DropColumn(
                name: "RoughSeaState",
                table: "sts_operations");

            migrationBuilder.AlterColumn<string>(
                name: "ParticipatingVessel",
                table: "sts_operations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<short>(
                name: "SeaState",
                table: "sts_operations",
                type: "smallint",
                nullable: true);
        }
    }
}
