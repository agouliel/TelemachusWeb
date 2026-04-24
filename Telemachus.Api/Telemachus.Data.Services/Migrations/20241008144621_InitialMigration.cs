using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Telemachus.Data.Services.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Operator = table.Column<string>(type: "varchar(50)", nullable: true),
                    PitchPropeller = table.Column<double>(nullable: true),
                    MainEngineMaxPower = table.Column<double>(nullable: true),
                    MainEngineAlarmPower = table.Column<int>(nullable: true),
                    AvailablePasscodeSlots = table.Column<int>(nullable: false),
                    Prefix = table.Column<string>(maxLength: 3, nullable: false),
                    RemoteAddress = table.Column<string>(maxLength: 255, nullable: true),
                    RemotePort = table.Column<int>(nullable: false),
                    NonPool = table.Column<bool>(nullable: false),
                    NonHafnia = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "document_types",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    BusinessId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "event_conditions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(type: "varchar(256)", nullable: false),
                    BusinessId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_conditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "event_statuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", nullable: true),
                    BusinessId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: false),
                    BusinessId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "report_field_groups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    FieldGroupName = table.Column<string>(type: "varchar(512)", nullable: false),
                    BusinessId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report_field_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "report_types",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", nullable: true),
                    BusinessId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tanks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(type: "varchar(512)", nullable: false),
                    SettlingService = table.Column<bool>(nullable: false),
                    BusinessId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPasscodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    Passcode = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasscodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPasscodes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "voyages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTimeOffset>(nullable: false),
                    EndDate = table.Column<DateTimeOffset>(nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsFinished = table.Column<bool>(nullable: false),
                    CurrentConditionId = table.Column<int>(nullable: false),
                    CurrentVoyageConditionKey = table.Column<Guid>(nullable: false),
                    BusinessId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voyages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_voyages_event_conditions_CurrentConditionId",
                        column: x => x.CurrentConditionId,
                        principalTable: "event_conditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_voyages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PortAreaCoordinates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(nullable: false),
                    Lng = table.Column<double>(nullable: false),
                    Lat = table.Column<double>(nullable: false),
                    PointIndex = table.Column<int>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    BusinessId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortAreaCoordinates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortAreaCoordinates_PortAreas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "PortAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortSeaRegions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    AreaId = table.Column<int>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: false),
                    BusinessId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortSeaRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortSeaRegions_PortAreas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "PortAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "event_types",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", nullable: true),
                    PairedEventTypeId = table.Column<int>(nullable: true),
                    NextConditionId = table.Column<int>(nullable: true),
                    Transit = table.Column<bool>(nullable: false, defaultValue: false),
                    AvailableAfterEventTypeId = table.Column<int>(nullable: true),
                    EventType = table.Column<int>(nullable: false),
                    ReportTypeId = table.Column<int>(nullable: true),
                    BusinessId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_types", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_types_event_types_AvailableAfterEventTypeId",
                        column: x => x.AvailableAfterEventTypeId,
                        principalTable: "event_types",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_event_types_event_conditions_NextConditionId",
                        column: x => x.NextConditionId,
                        principalTable: "event_conditions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_event_types_event_types_PairedEventTypeId",
                        column: x => x.PairedEventTypeId,
                        principalTable: "event_types",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_event_types_report_types_ReportTypeId",
                        column: x => x.ReportTypeId,
                        principalTable: "report_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "report_fields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", nullable: true),
                    IsSubgroupMain = table.Column<bool>(nullable: true),
                    Group = table.Column<int>(nullable: true),
                    TankId = table.Column<int>(nullable: true),
                    ValidationKey = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    BusinessId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report_fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_report_fields_report_field_groups_Group",
                        column: x => x.Group,
                        principalTable: "report_field_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_report_fields_tanks_TankId",
                        column: x => x.TankId,
                        principalTable: "tanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tank_user_specs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    TankId = table.Column<int>(nullable: false),
                    MaxCapacity = table.Column<string>(type: "varchar(512)", nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    TankName = table.Column<string>(nullable: true),
                    BusinessId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tank_user_specs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tank_user_specs_tanks_TankId",
                        column: x => x.TankId,
                        principalTable: "tanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tank_user_specs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortCountries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numerical = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Alpha2 = table.Column<string>(nullable: true),
                    Alpha3 = table.Column<string>(nullable: true),
                    Nationality = table.Column<string>(nullable: true),
                    RegionId = table.Column<int>(nullable: true),
                    LloydsCode = table.Column<string>(nullable: true),
                    PhoneCode = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: false),
                    BusinessId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortCountries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortCountries_PortSeaRegions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "PortSeaRegions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "event_types_conditions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ConditionId = table.Column<int>(nullable: false),
                    EventTypeId = table.Column<int>(nullable: false),
                    BusinessId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_types_conditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_types_conditions_event_conditions_ConditionId",
                        column: x => x.ConditionId,
                        principalTable: "event_conditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_types_conditions_event_types_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "event_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "report_field_relations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ReportFieldId = table.Column<int>(nullable: false),
                    ReportTypeId = table.Column<int>(nullable: false),
                    BusinessId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report_field_relations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_report_field_relations_report_fields_ReportFieldId",
                        column: x => x.ReportFieldId,
                        principalTable: "report_fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_report_field_relations_report_types_ReportTypeId",
                        column: x => x.ReportTypeId,
                        principalTable: "report_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Latitude = table.Column<decimal>(type: "DECIMAL(9,6)", nullable: true),
                    Longitude = table.Column<decimal>(type: "DECIMAL(9,6)", nullable: true),
                    Code = table.Column<string>(nullable: true),
                    CountryId = table.Column<int>(nullable: false),
                    RegionId = table.Column<int>(nullable: false),
                    AreaId = table.Column<int>(nullable: false),
                    TimeZone = table.Column<double>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: false),
                    BusinessId = table.Column<string>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ports_PortAreas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "PortAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ports_PortCountries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "PortCountries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ports_PortSeaRegions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "PortSeaRegions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.Sql(@"
            ALTER TABLE [Ports] ADD [Point] AS 
                (CASE WHEN [Latitude] IS NULL OR [Longitude] IS NULL 
                      THEN NULL 
                      ELSE GEOGRAPHY::Point([Latitude], [Longitude], 4326) 
                 END) PERSISTED;
        ");

            migrationBuilder.Sql(@"
            CREATE SPATIAL INDEX IX_Ports_Point 
            ON [Ports]([Point])
            USING GEOGRAPHY_GRID;
        ");
            migrationBuilder.CreateTable(
                name: "bunkering_data",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Bdn = table.Column<string>(nullable: true),
                    SulphurContent = table.Column<string>(nullable: true),
                    Density = table.Column<string>(nullable: true),
                    Viscosity = table.Column<string>(nullable: true),
                    FuelType = table.Column<int>(nullable: false),
                    Supplier = table.Column<string>(nullable: true),
                    TotalAmount = table.Column<string>(nullable: true),
                    PortId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    BusinessId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bunkering_data", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bunkering_data_Ports_PortId",
                        column: x => x.PortId,
                        principalTable: "Ports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bunkering_data_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "statement_of_facts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    FromDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ToDate = table.Column<DateTime>(type: "Date", nullable: false),
                    LastEventId = table.Column<int>(nullable: true),
                    FirstEventId = table.Column<int>(nullable: true),
                    Completed = table.Column<bool>(nullable: false, defaultValue: false),
                    Date = table.Column<DateTime>(type: "Date", nullable: true),
                    OperationGrade = table.Column<string>(nullable: true),
                    Voyage = table.Column<string>(nullable: true),
                    PortId = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Terminal = table.Column<string>(nullable: true),
                    CharterParty = table.Column<string>(nullable: true),
                    BusinessId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statement_of_facts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_statement_of_facts_Ports_PortId",
                        column: x => x.PortId,
                        principalTable: "Ports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_statement_of_facts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bunkering_tank_data",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    BunkeringDataId = table.Column<int>(nullable: false),
                    TankId = table.Column<int>(nullable: false),
                    Amount = table.Column<string>(nullable: true),
                    ComminglingId = table.Column<int>(nullable: true),
                    BusinessId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bunkering_tank_data", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bunkering_tank_data_bunkering_data_BunkeringDataId",
                        column: x => x.BunkeringDataId,
                        principalTable: "bunkering_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bunkering_tank_data_bunkering_data_ComminglingId",
                        column: x => x.ComminglingId,
                        principalTable: "bunkering_data",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_bunkering_tank_data_tanks_TankId",
                        column: x => x.TankId,
                        principalTable: "tanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    BusinessId = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Terminal = table.Column<string>(type: "varchar(128)", nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    EventTypeId = table.Column<int>(nullable: false),
                    ConditionId = table.Column<int>(nullable: false),
                    VoyageId = table.Column<int>(nullable: false),
                    CurrentVoyageConditionKey = table.Column<Guid>(nullable: false),
                    NextVoyageConditionKey = table.Column<Guid>(nullable: true),
                    PreviousVoyageConditionKey = table.Column<Guid>(nullable: true),
                    ChangingConditionEventAvailableAfterEventId = table.Column<int>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    ConditionStartedDate = table.Column<DateTimeOffset>(nullable: true),
                    BallastQuantity = table.Column<decimal>(nullable: true),
                    ParentEventId = table.Column<int>(nullable: true),
                    PortId = table.Column<int>(nullable: true),
                    CustomEventName = table.Column<string>(type: "varchar(128)", nullable: true),
                    ExcludeFromStatement = table.Column<bool>(nullable: false, defaultValue: false),
                    HiddenDate = table.Column<bool>(nullable: false),
                    LatDegrees = table.Column<int>(nullable: true),
                    LatMinutes = table.Column<int>(nullable: true),
                    LatSeconds = table.Column<int>(nullable: true),
                    LongDegrees = table.Column<int>(nullable: true),
                    LongMinutes = table.Column<int>(nullable: true),
                    LongSeconds = table.Column<int>(nullable: true),
                    DebugData = table.Column<string>(nullable: true),
                    BunkeringDataId = table.Column<int>(nullable: true),
                    Lat = table.Column<decimal>(type: "DECIMAL(9,6)", nullable: true),
                    Lng = table.Column<decimal>(type: "DECIMAL(9,6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_events_bunkering_data_BunkeringDataId",
                        column: x => x.BunkeringDataId,
                        principalTable: "bunkering_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_events_events_ChangingConditionEventAvailableAfterEventId",
                        column: x => x.ChangingConditionEventAvailableAfterEventId,
                        principalTable: "events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_events_event_conditions_ConditionId",
                        column: x => x.ConditionId,
                        principalTable: "event_conditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_events_event_types_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "event_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_events_events_ParentEventId",
                        column: x => x.ParentEventId,
                        principalTable: "events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_events_Ports_PortId",
                        column: x => x.PortId,
                        principalTable: "Ports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_events_event_statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "event_statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_events_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_events_voyages_VoyageId",
                        column: x => x.VoyageId,
                        principalTable: "voyages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.Sql(@"
            ALTER TABLE [events] ADD [Point] AS 
                (CASE WHEN [Lat] IS NULL OR [Lng] IS NULL 
                      THEN NULL 
                      ELSE GEOGRAPHY::Point([Lat], [Lng], 4326) 
                 END) PERSISTED;
            ");

            migrationBuilder.Sql(@"
            CREATE SPATIAL INDEX IX_events_Point 
            ON [events]([Point])
            USING GEOGRAPHY_GRID;
            ");
            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    BusinessId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reports_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_attachments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    BusinessId = table.Column<string>(nullable: false),
                    ReportId = table.Column<int>(nullable: true),
                    ReportFieldId = table.Column<int>(nullable: true),
                    FileName = table.Column<string>(maxLength: 255, nullable: true),
                    MimeType = table.Column<string>(maxLength: 255, nullable: true),
                    FileSize = table.Column<long>(nullable: false),
                    DocumentTypeId = table.Column<int>(nullable: true),
                    BunkeringDataId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_attachments_bunkering_data_BunkeringDataId",
                        column: x => x.BunkeringDataId,
                        principalTable: "bunkering_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_event_attachments_document_types_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "document_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_event_attachments_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_attachments_report_fields_ReportFieldId",
                        column: x => x.ReportFieldId,
                        principalTable: "report_fields",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_event_attachments_reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "reports",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "report_field_values",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Value = table.Column<string>(type: "varchar(512)", nullable: true),
                    ReportId = table.Column<int>(nullable: false),
                    ReportFieldId = table.Column<int>(nullable: false),
                    BusinessId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report_field_values", x => x.Id);
                    table.ForeignKey(
                        name: "FK_report_field_values_report_fields_ReportFieldId",
                        column: x => x.ReportFieldId,
                        principalTable: "report_fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_report_field_values_reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Prefix",
                table: "AspNetUsers",
                column: "Prefix",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bunkering_data_BusinessId",
                table: "bunkering_data",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bunkering_data_DateModified",
                table: "bunkering_data",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_bunkering_data_IsDeleted",
                table: "bunkering_data",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_bunkering_data_PortId",
                table: "bunkering_data",
                column: "PortId");

            migrationBuilder.CreateIndex(
                name: "IX_bunkering_data_UserId",
                table: "bunkering_data",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_bunkering_tank_data_BusinessId",
                table: "bunkering_tank_data",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bunkering_tank_data_ComminglingId",
                table: "bunkering_tank_data",
                column: "ComminglingId");

            migrationBuilder.CreateIndex(
                name: "IX_bunkering_tank_data_DateModified",
                table: "bunkering_tank_data",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_bunkering_tank_data_IsDeleted",
                table: "bunkering_tank_data",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_bunkering_tank_data_TankId",
                table: "bunkering_tank_data",
                column: "TankId");

            migrationBuilder.CreateIndex(
                name: "IX_bunkering_tank_data_BunkeringDataId_TankId",
                table: "bunkering_tank_data",
                columns: new[] { "BunkeringDataId", "TankId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_document_types_BusinessId",
                table: "document_types",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_document_types_DateModified",
                table: "document_types",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_document_types_IsDeleted",
                table: "document_types",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_event_attachments_BunkeringDataId",
                table: "event_attachments",
                column: "BunkeringDataId");

            migrationBuilder.CreateIndex(
                name: "IX_event_attachments_BusinessId",
                table: "event_attachments",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_attachments_DateModified",
                table: "event_attachments",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_event_attachments_DocumentTypeId",
                table: "event_attachments",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_event_attachments_EventId",
                table: "event_attachments",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_event_attachments_IsDeleted",
                table: "event_attachments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_event_attachments_ReportFieldId",
                table: "event_attachments",
                column: "ReportFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_event_attachments_ReportId",
                table: "event_attachments",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_event_conditions_BusinessId",
                table: "event_conditions",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_conditions_DateModified",
                table: "event_conditions",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_event_conditions_IsDeleted",
                table: "event_conditions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_event_statuses_BusinessId",
                table: "event_statuses",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_statuses_DateModified",
                table: "event_statuses",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_event_statuses_IsDeleted",
                table: "event_statuses",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_event_types_AvailableAfterEventTypeId",
                table: "event_types",
                column: "AvailableAfterEventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_event_types_BusinessId",
                table: "event_types",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_types_DateModified",
                table: "event_types",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_event_types_IsDeleted",
                table: "event_types",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_event_types_NextConditionId",
                table: "event_types",
                column: "NextConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_event_types_PairedEventTypeId",
                table: "event_types",
                column: "PairedEventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_event_types_ReportTypeId",
                table: "event_types",
                column: "ReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_event_types_conditions_BusinessId",
                table: "event_types_conditions",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_types_conditions_ConditionId",
                table: "event_types_conditions",
                column: "ConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_event_types_conditions_DateModified",
                table: "event_types_conditions",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_event_types_conditions_EventTypeId",
                table: "event_types_conditions",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_event_types_conditions_IsDeleted",
                table: "event_types_conditions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_events_BunkeringDataId",
                table: "events",
                column: "BunkeringDataId");

            migrationBuilder.CreateIndex(
                name: "IX_events_BusinessId",
                table: "events",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_events_ChangingConditionEventAvailableAfterEventId",
                table: "events",
                column: "ChangingConditionEventAvailableAfterEventId");

            migrationBuilder.CreateIndex(
                name: "IX_events_ConditionId",
                table: "events",
                column: "ConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_events_DateModified",
                table: "events",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_events_EventTypeId",
                table: "events",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_events_IsDeleted",
                table: "events",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_events_ParentEventId",
                table: "events",
                column: "ParentEventId");

            migrationBuilder.CreateIndex(
                name: "IX_events_PortId",
                table: "events",
                column: "PortId");

            migrationBuilder.CreateIndex(
                name: "IX_events_StatusId",
                table: "events",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_events_Timestamp",
                table: "events",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_events_UserId",
                table: "events",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_events_VoyageId",
                table: "events",
                column: "VoyageId");

            migrationBuilder.CreateIndex(
                name: "IX_PortAreaCoordinates_AreaId",
                table: "PortAreaCoordinates",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_PortAreaCoordinates_BusinessId",
                table: "PortAreaCoordinates",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortAreaCoordinates_DateModified",
                table: "PortAreaCoordinates",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_PortAreas_BusinessId",
                table: "PortAreas",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortAreas_DateModified",
                table: "PortAreas",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_PortCountries_BusinessId",
                table: "PortCountries",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortCountries_DateModified",
                table: "PortCountries",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_PortCountries_RegionId",
                table: "PortCountries",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Ports_AreaId",
                table: "Ports",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ports_BusinessId",
                table: "Ports",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ports_CountryId",
                table: "Ports",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Ports_DateModified",
                table: "Ports",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_Ports_RegionId",
                table: "Ports",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_PortSeaRegions_AreaId",
                table: "PortSeaRegions",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_PortSeaRegions_BusinessId",
                table: "PortSeaRegions",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortSeaRegions_DateModified",
                table: "PortSeaRegions",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_report_field_groups_BusinessId",
                table: "report_field_groups",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_report_field_groups_DateModified",
                table: "report_field_groups",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_report_field_groups_IsDeleted",
                table: "report_field_groups",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_report_field_relations_BusinessId",
                table: "report_field_relations",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_report_field_relations_DateModified",
                table: "report_field_relations",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_report_field_relations_IsDeleted",
                table: "report_field_relations",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_report_field_relations_ReportTypeId",
                table: "report_field_relations",
                column: "ReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_report_field_relations_ReportFieldId_ReportTypeId",
                table: "report_field_relations",
                columns: new[] { "ReportFieldId", "ReportTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_report_field_values_BusinessId",
                table: "report_field_values",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_report_field_values_DateModified",
                table: "report_field_values",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_report_field_values_IsDeleted",
                table: "report_field_values",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_report_field_values_ReportFieldId",
                table: "report_field_values",
                column: "ReportFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_report_field_values_ReportId",
                table: "report_field_values",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_report_fields_BusinessId",
                table: "report_fields",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_report_fields_DateModified",
                table: "report_fields",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_report_fields_Group",
                table: "report_fields",
                column: "Group");

            migrationBuilder.CreateIndex(
                name: "IX_report_fields_IsDeleted",
                table: "report_fields",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_report_fields_TankId",
                table: "report_fields",
                column: "TankId");

            migrationBuilder.CreateIndex(
                name: "IX_report_types_BusinessId",
                table: "report_types",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_report_types_DateModified",
                table: "report_types",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_report_types_IsDeleted",
                table: "report_types",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_reports_BusinessId",
                table: "reports",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reports_DateModified",
                table: "reports",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_reports_EventId",
                table: "reports",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_reports_IsDeleted",
                table: "reports",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_statement_of_facts_BusinessId",
                table: "statement_of_facts",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_statement_of_facts_DateModified",
                table: "statement_of_facts",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_statement_of_facts_IsDeleted",
                table: "statement_of_facts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_statement_of_facts_PortId",
                table: "statement_of_facts",
                column: "PortId");

            migrationBuilder.CreateIndex(
                name: "IX_statement_of_facts_UserId",
                table: "statement_of_facts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tank_user_specs_BusinessId",
                table: "tank_user_specs",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tank_user_specs_DateModified",
                table: "tank_user_specs",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_tank_user_specs_IsDeleted",
                table: "tank_user_specs",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_tank_user_specs_TankId",
                table: "tank_user_specs",
                column: "TankId");

            migrationBuilder.CreateIndex(
                name: "IX_tank_user_specs_UserId_TankId",
                table: "tank_user_specs",
                columns: new[] { "UserId", "TankId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tanks_BusinessId",
                table: "tanks",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tanks_DateModified",
                table: "tanks",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_tanks_IsDeleted",
                table: "tanks",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_tanks_Id_Name",
                table: "tanks",
                columns: new[] { "Id", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPasscodes_UserId",
                table: "UserPasscodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_voyages_BusinessId",
                table: "voyages",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_voyages_CurrentConditionId",
                table: "voyages",
                column: "CurrentConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_voyages_DateModified",
                table: "voyages",
                column: "DateModified");

            migrationBuilder.CreateIndex(
                name: "IX_voyages_IsDeleted",
                table: "voyages",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_voyages_UserId",
                table: "voyages",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "bunkering_tank_data");

            migrationBuilder.DropTable(
                name: "event_attachments");

            migrationBuilder.DropTable(
                name: "event_types_conditions");

            migrationBuilder.DropTable(
                name: "PortAreaCoordinates");

            migrationBuilder.DropTable(
                name: "report_field_relations");

            migrationBuilder.DropTable(
                name: "report_field_values");

            migrationBuilder.DropTable(
                name: "statement_of_facts");

            migrationBuilder.DropTable(
                name: "tank_user_specs");

            migrationBuilder.DropTable(
                name: "UserPasscodes");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "document_types");

            migrationBuilder.DropTable(
                name: "report_fields");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "report_field_groups");

            migrationBuilder.DropTable(
                name: "tanks");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "bunkering_data");

            migrationBuilder.DropTable(
                name: "event_types");

            migrationBuilder.DropTable(
                name: "event_statuses");

            migrationBuilder.DropTable(
                name: "voyages");

            migrationBuilder.DropTable(
                name: "Ports");

            migrationBuilder.DropTable(
                name: "report_types");

            migrationBuilder.DropTable(
                name: "event_conditions");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PortCountries");

            migrationBuilder.DropTable(
                name: "PortSeaRegions");

            migrationBuilder.DropTable(
                name: "PortAreas");
        }
    }
}
