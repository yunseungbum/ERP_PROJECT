using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace BuddyErp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddExpensesAndSchedulePayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "payer_name",
                table: "match_schedules",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "윤승범");

            migrationBuilder.CreateTable(
                name: "expenses",
                columns: table => new
                {
                    expense_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    schedule_id = table.Column<long>(type: "bigint", nullable: true),
                    expense_item = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    payment_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    notes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    payer_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    is_settled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expenses", x => x.expense_id);
                    table.ForeignKey(
                        name: "FK_expenses_match_schedules_schedule_id",
                        column: x => x.schedule_id,
                        principalTable: "match_schedules",
                        principalColumn: "schedule_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "match_schedules",
                keyColumn: "schedule_id",
                keyValue: 1L,
                column: "payer_name",
                value: "윤승범");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_schedule_id",
                table: "expenses",
                column: "schedule_id",
                unique: true);

            migrationBuilder.Sql(
                """
                UPDATE match_schedules
                SET payer_name = '윤승범'
                WHERE payer_name = '';

                INSERT INTO expenses (
                    schedule_id,
                    expense_item,
                    amount,
                    payment_date,
                    notes,
                    payer_name,
                    is_settled,
                    created_at,
                    updated_at
                )
                SELECT
                    schedule_id,
                    '구장비',
                    match_fee,
                    starts_at,
                    CONCAT(
                        DATE_FORMAT(starts_at, '%Y.%m.%d'),
                        ' ',
                        venue_name,
                        ' ',
                        DATE_FORMAT(starts_at, '%H:%i')
                    ),
                    payer_name,
                    is_match_fee_paid,
                    created_at,
                    updated_at
                FROM match_schedules;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "expenses");

            migrationBuilder.DropColumn(
                name: "payer_name",
                table: "match_schedules");
        }
    }
}
