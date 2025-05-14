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
                    HabitTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Factor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitTypes", x => x.HabitTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetTable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    FootprintSummaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalFootprint = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GeneratedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    UserChoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HabitTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Footprint = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                columns: new[] { "HabitTypeId", "Factor", "Name", "Unit" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 0.192m, "Deslocação de carro (gasolina)", "km" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 0.041m, "Viagem de comboio", "km" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), 0.255m, "Viagem de avião", "km" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), 0.233m, "Consumo de eletricidade", "kWh" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), 5.0m, "Refeição com carne", "unidade" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), 2.0m, "Refeição vegetariana", "unidade" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), 350m, "Compra de bens eletrónicos", "unidade" },
                    { new Guid("88888888-8888-8888-8888-888888888888"), 0.3m, "Banho quente (10 min)", "minuto" },
                    { new Guid("99999999-9999-9999-9999-999999999999"), -1.8m, "Reciclagem de plástico", "kg" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), -21m, "Plantação de árvores", "unidade" }
                });

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
