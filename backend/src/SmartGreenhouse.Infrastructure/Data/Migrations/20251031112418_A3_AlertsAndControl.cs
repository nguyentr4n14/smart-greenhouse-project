using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SmartGreenhouse.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class A3_AlertsAndControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Readings_Devices_DeviceId",
                table: "Readings");

            migrationBuilder.DropIndex(
                name: "IX_Devices_DeviceType",
                table: "Devices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Readings",
                table: "Readings");

            migrationBuilder.RenameTable(
                name: "Readings",
                newName: "SensorReadings");

            migrationBuilder.RenameIndex(
                name: "IX_Readings_DeviceId_SensorType_Timestamp",
                table: "SensorReadings",
                newName: "IX_SensorReadings_DeviceId_SensorType_Timestamp");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceName",
                table: "Devices",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AddColumn<int>(
                name: "DeviceId1",
                table: "SensorReadings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SensorReadings",
                table: "SensorReadings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AlertRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    SensorType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OperatorSymbol = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Threshold = table.Column<double>(type: "double precision", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertRules_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ControlProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    StrategyKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ParametersJson = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ControlProfiles_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlertNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlertRuleId = table.Column<int>(type: "integer", nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    SensorType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    Threshold = table.Column<double>(type: "double precision", nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TriggeredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertNotifications_AlertRules_AlertRuleId",
                        column: x => x.AlertRuleId,
                        principalTable: "AlertRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AlertNotifications_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_DeviceId1",
                table: "SensorReadings",
                column: "DeviceId1");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_Timestamp",
                table: "SensorReadings",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AlertNotifications_AlertRuleId",
                table: "AlertNotifications",
                column: "AlertRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertNotifications_DeviceId_TriggeredAt",
                table: "AlertNotifications",
                columns: new[] { "DeviceId", "TriggeredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AlertRules_DeviceId_SensorType_IsActive",
                table: "AlertRules",
                columns: new[] { "DeviceId", "SensorType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ControlProfiles_DeviceId",
                table: "ControlProfiles",
                column: "DeviceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SensorReadings_Devices_DeviceId",
                table: "SensorReadings",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SensorReadings_Devices_DeviceId1",
                table: "SensorReadings",
                column: "DeviceId1",
                principalTable: "Devices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorReadings_Devices_DeviceId",
                table: "SensorReadings");

            migrationBuilder.DropForeignKey(
                name: "FK_SensorReadings_Devices_DeviceId1",
                table: "SensorReadings");

            migrationBuilder.DropTable(
                name: "AlertNotifications");

            migrationBuilder.DropTable(
                name: "ControlProfiles");

            migrationBuilder.DropTable(
                name: "AlertRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SensorReadings",
                table: "SensorReadings");

            migrationBuilder.DropIndex(
                name: "IX_SensorReadings_DeviceId1",
                table: "SensorReadings");

            migrationBuilder.DropIndex(
                name: "IX_SensorReadings_Timestamp",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "DeviceId1",
                table: "SensorReadings");

            migrationBuilder.RenameTable(
                name: "SensorReadings",
                newName: "Readings");

            migrationBuilder.RenameIndex(
                name: "IX_SensorReadings_DeviceId_SensorType_Timestamp",
                table: "Readings",
                newName: "IX_Readings_DeviceId_SensorType_Timestamp");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceName",
                table: "Devices",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Readings",
                table: "Readings",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceType",
                table: "Devices",
                column: "DeviceType");

            migrationBuilder.AddForeignKey(
                name: "FK_Readings_Devices_DeviceId",
                table: "Readings",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
