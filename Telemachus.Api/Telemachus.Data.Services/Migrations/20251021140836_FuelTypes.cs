using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class FuelTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fuel_types",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    BusinessId = table.Column<Guid>(nullable: false, defaultValueSql: "newid()"),
                    Name = table.Column<string>(type: "varchar(512)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fuel_types", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "fuel_types",
                columns: new[] { "Id", "BusinessId", "Name" },
                values: new object[] { 1, new Guid("3d02e784-93d8-41d9-b240-0fc284c2067e"), "HFO" });

            migrationBuilder.InsertData(
                table: "fuel_types",
                columns: new[] { "Id", "BusinessId", "Name" },
                values: new object[] { 2, new Guid("7f5f98e9-a61e-4c9f-a3f6-415a317c5030"), "MGO" });

            migrationBuilder.CreateIndex(
                name: "IX_fuel_types_BusinessId",
                table: "fuel_types",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fuel_types_DateModified",
                table: "fuel_types",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_types_IsDeleted",
                table: "fuel_types",
                column: "IsDeleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fuel_types");
        }
    }
}
