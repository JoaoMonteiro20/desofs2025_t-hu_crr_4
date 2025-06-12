using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EcoImpact.API.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HabitTypes",
                columns: table => new
                {
                    HabitTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Factor = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitTypes", x => x.HabitTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    EcoScore = table.Column<decimal>(type: "numeric", nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                    LockoutEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionType = table.Column<string>(type: "text", nullable: false),
                    TargetTable = table.Column<string>(type: "text", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FootprintSummaries",
                columns: table => new
                {
                    FootprintSummaryId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalFootprint = table.Column<decimal>(type: "numeric", nullable: false),
                    GeneratedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FootprintSummaries", x => x.FootprintSummaryId);
                    table.ForeignKey(
                        name: "FK_FootprintSummaries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserChoices",
                columns: table => new
                {
                    UserChoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    HabitTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Footprint = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChoices", x => x.UserChoiceId);
                    table.ForeignKey(
                        name: "FK_UserChoices_HabitTypes_HabitTypeId",
                        column: x => x.HabitTypeId,
                        principalTable: "HabitTypes",
                        principalColumn: "HabitTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserChoices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "HabitTypes",
                columns: new[] { "HabitTypeId", "Category", "Factor", "Name", "Unit" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 0, 0.192m, "Deslocação de carro (gasolina)", "km" },
                    { new Guid("11111111-1111-1111-1111-111111111112"), 0, 0.041m, "Viagem de comboio", "km" },
                    { new Guid("11111111-1111-1111-1111-111111111113"), 0, 0.255m, "Viagem de avião", "km" },
                    { new Guid("22222222-2222-2222-2222-222222222221"), 1, 5.0m, "Refeição com carne", "unidade" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 1, 2.0m, "Refeição vegetariana", "unidade" },
                    { new Guid("22222222-2222-2222-2222-222222222223"), 1, 1.5m, "Refeição vegan", "unidade" },
                    { new Guid("33333333-3333-3333-3333-333333333331"), 2, 0.233m, "Consumo de eletricidade", "kWh" },
                    { new Guid("33333333-3333-3333-3333-333333333332"), 2, 0.3m, "Banho quente (10 min)", "minuto" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), 2, 1.2m, "Uso de aquecedor elétrico", "hora" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "EcoScore", "Email", "FailedLoginAttempts", "LockoutEnd", "Password", "Role", "UserName" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), 0m, "admin@ecoimpact.local", 0, null, "AQAAAAIAAYagAAAAEEISJn23wqjANxH/pmq3ug2f+MTVEF+p5yB7TORYNv6wFmeRVaTTL1G1objmD/A9Dg==", 0, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FootprintSummaries_UserId",
                table: "FootprintSummaries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChoices_HabitTypeId",
                table: "UserChoices",
                column: "HabitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChoices_UserId",
                table: "UserChoices",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "FootprintSummaries");

            migrationBuilder.DropTable(
                name: "UserChoices");

            migrationBuilder.DropTable(
                name: "HabitTypes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
