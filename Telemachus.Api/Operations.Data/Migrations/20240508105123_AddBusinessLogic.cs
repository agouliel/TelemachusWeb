using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Operations.Data.Migrations
{
    public partial class AddBusinessLogic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessId",
                table: "SeaRegions",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified2",
                table: "SeaRegions",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "BusinessId",
                table: "PortsV2",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified2",
                table: "PortsV2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "BusinessId",
                table: "Countries",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified2",
                table: "Countries",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "BusinessId",
                table: "Areas",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified2",
                table: "Areas",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "BusinessId",
                table: "AreaCoordinates",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified2",
                table: "AreaCoordinates",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.CreateIndex(
                name: "IX_SeaRegions_BusinessId",
                table: "SeaRegions",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeaRegions_DateModified2",
                table: "SeaRegions",
                column: "DateModified2");

            migrationBuilder.CreateIndex(
                name: "IX_PortsV2_BusinessId",
                table: "PortsV2",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortsV2_DateModified2",
                table: "PortsV2",
                column: "DateModified2");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_BusinessId",
                table: "Countries",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_DateModified2",
                table: "Countries",
                column: "DateModified2");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_BusinessId",
                table: "Areas",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Areas_DateModified2",
                table: "Areas",
                column: "DateModified2");

            migrationBuilder.CreateIndex(
                name: "IX_AreaCoordinates_BusinessId",
                table: "AreaCoordinates",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AreaCoordinates_DateModified2",
                table: "AreaCoordinates",
                column: "DateModified2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SeaRegions_BusinessId",
                table: "SeaRegions");

            migrationBuilder.DropIndex(
                name: "IX_SeaRegions_DateModified2",
                table: "SeaRegions");

            migrationBuilder.DropIndex(
                name: "IX_PortsV2_BusinessId",
                table: "PortsV2");

            migrationBuilder.DropIndex(
                name: "IX_PortsV2_DateModified2",
                table: "PortsV2");

            migrationBuilder.DropIndex(
                name: "IX_Countries_BusinessId",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Countries_DateModified2",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Areas_BusinessId",
                table: "Areas");

            migrationBuilder.DropIndex(
                name: "IX_Areas_DateModified2",
                table: "Areas");

            migrationBuilder.DropIndex(
                name: "IX_AreaCoordinates_BusinessId",
                table: "AreaCoordinates");

            migrationBuilder.DropIndex(
                name: "IX_AreaCoordinates_DateModified2",
                table: "AreaCoordinates");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "SeaRegions");

            migrationBuilder.DropColumn(
                name: "DateModified2",
                table: "SeaRegions");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "PortsV2");

            migrationBuilder.DropColumn(
                name: "DateModified2",
                table: "PortsV2");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "DateModified2",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "DateModified2",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "AreaCoordinates");

            migrationBuilder.DropColumn(
                name: "DateModified2",
                table: "AreaCoordinates");
        }
    }
}
