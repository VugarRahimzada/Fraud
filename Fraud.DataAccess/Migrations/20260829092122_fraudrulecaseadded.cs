using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fraud.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class fraudrulecaseadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FraudEvaluatedAt",
                table: "Transactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RiskLevel",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FraudCaseRuleResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FraudCaseId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    RuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RiskScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudCaseRuleResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FraudCaseRuleResults_FraudCases_FraudCaseId",
                        column: x => x.FraudCaseId,
                        principalTable: "FraudCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FraudCaseRuleResults_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FraudCaseRuleResults_FraudCaseId",
                table: "FraudCaseRuleResults",
                column: "FraudCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_FraudCaseRuleResults_TransactionId",
                table: "FraudCaseRuleResults",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FraudCaseRuleResults");

            migrationBuilder.DropColumn(
                name: "FraudEvaluatedAt",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RiskLevel",
                table: "Transactions");
        }
    }
}
