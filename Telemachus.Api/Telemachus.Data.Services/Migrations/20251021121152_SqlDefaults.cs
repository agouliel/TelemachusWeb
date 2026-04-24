using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemachus.Data.Services.Migrations
{
    public partial class SqlDefaults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "voyages",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "Storage",
                table: "tanks",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit],(1))");

            migrationBuilder.AlterColumn<bool>(
                name: "Settling",
                table: "tanks",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit],(0))");

            migrationBuilder.AlterColumn<bool>(
                name: "Serving",
                table: "tanks",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit],(0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "tanks",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "tank_user_specs",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "tank_user_specs",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit],(1))");

            migrationBuilder.AlterColumn<int>(
                name: "DisplayOrder",
                table: "tank_user_specs",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "0");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "statement_of_facts",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "reports",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "report_types",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "report_fields",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "report_field_values",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "report_field_relations",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "report_field_groups",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EventTypePrerequisites",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "events",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "HiddenDate",
                table: "events",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit],(0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "event_types_conditions",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "event_types",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "event_statuses",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "event_conditions",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "event_attachments",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<long>(
                name: "FileSize",
                table: "event_attachments",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValueSql: "CONVERT([bigint],(0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "document_types",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "cargoes",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "cargo_grades",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "cargo_details",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "bunkering_tank_data",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "bunkering_data",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "CONVERT([bit], (0))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "voyages",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Storage",
                table: "tanks",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit],(1))",
                oldClrType: typeof(bool),
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Settling",
                table: "tanks",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit],(0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Serving",
                table: "tanks",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit],(0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "tanks",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "tank_user_specs",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "tank_user_specs",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit],(1))",
                oldClrType: typeof(bool),
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<int>(
                name: "DisplayOrder",
                table: "tank_user_specs",
                type: "int",
                nullable: false,
                defaultValueSql: "0",
                oldClrType: typeof(int),
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "statement_of_facts",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "reports",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "report_types",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "report_fields",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "report_field_values",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "report_field_relations",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "report_field_groups",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EventTypePrerequisites",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "events",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "HiddenDate",
                table: "events",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit],(0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "event_types_conditions",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "event_types",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "event_statuses",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "event_conditions",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "event_attachments",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<long>(
                name: "FileSize",
                table: "event_attachments",
                type: "bigint",
                nullable: false,
                defaultValueSql: "CONVERT([bigint],(0))",
                oldClrType: typeof(long),
                oldDefaultValue: 0L);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "document_types",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "cargoes",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "cargo_grades",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "cargo_details",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "bunkering_tank_data",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "bunkering_data",
                type: "bit",
                nullable: false,
                defaultValueSql: "CONVERT([bit], (0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);
        }
    }
}
