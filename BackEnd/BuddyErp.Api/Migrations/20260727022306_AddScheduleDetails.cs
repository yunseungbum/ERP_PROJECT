using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuddyErp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_completed",
                table: "match_schedules",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_match_fee_paid",
                table: "match_schedules",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "match_fee",
                table: "match_schedules",
                type: "decimal(12,0)",
                precision: 12,
                scale: 0,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "match_schedules",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "opponent_contact",
                table: "match_schedules",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "match_schedules",
                keyColumn: "schedule_id",
                keyValue: 1L,
                columns: new[] { "match_fee", "notes", "opponent_contact" },
                values: new object[] { 0m, "", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_completed",
                table: "match_schedules");

            migrationBuilder.DropColumn(
                name: "is_match_fee_paid",
                table: "match_schedules");

            migrationBuilder.DropColumn(
                name: "match_fee",
                table: "match_schedules");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "match_schedules");

            migrationBuilder.DropColumn(
                name: "opponent_contact",
                table: "match_schedules");
        }
    }
}
