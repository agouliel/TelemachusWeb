using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class Cargo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CargoDetailId",
                table: "events",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "cargo_grades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    BusinessId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cargo_grades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cargoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    GradeId = table.Column<int>(nullable: false),
                    Parcel = table.Column<int>(nullable: false),
                    StartedOn = table.Column<DateTimeOffset>(nullable: true),
                    CompletedOn = table.Column<DateTimeOffset>(nullable: true),
                    BusinessId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cargoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cargoes_cargo_grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "cargo_grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cargoes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cargo_details",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CargoId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    BusinessId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cargo_details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cargo_details_cargoes_CargoId",
                        column: x => x.CargoId,
                        principalTable: "cargoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_events_CargoDetailId",
                table: "events",
                column: "CargoDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_cargo_details_BusinessId",
                table: "cargo_details",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cargo_details_CargoId",
                table: "cargo_details",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_cargo_details_DateModified",
                table: "cargo_details",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_cargo_details_IsDeleted",
                table: "cargo_details",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_cargo_details_Timestamp",
                table: "cargo_details",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_cargo_grades_BusinessId",
                table: "cargo_grades",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cargo_grades_DateModified",
                table: "cargo_grades",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_cargo_grades_IsDeleted",
                table: "cargo_grades",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_cargoes_BusinessId",
                table: "cargoes",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cargoes_CompletedOn",
                table: "cargoes",
                column: "CompletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_cargoes_DateModified",
                table: "cargoes",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_cargoes_GradeId",
                table: "cargoes",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_cargoes_IsDeleted",
                table: "cargoes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_cargoes_StartedOn",
                table: "cargoes",
                column: "StartedOn");

            migrationBuilder.CreateIndex(
                name: "IX_cargoes_UserId",
                table: "cargoes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_events_cargo_details_CargoDetailId",
                table: "events",
                column: "CargoDetailId",
                principalTable: "cargo_details",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_events_cargo_details_CargoDetailId",
                table: "events");

            migrationBuilder.DropTable(
                name: "cargo_details");

            migrationBuilder.DropTable(
                name: "cargoes");

            migrationBuilder.DropTable(
                name: "cargo_grades");

            migrationBuilder.DropIndex(
                name: "IX_events_CargoDetailId",
                table: "events");

            migrationBuilder.DropColumn(
                name: "CargoDetailId",
                table: "events");
        }
    }
}
