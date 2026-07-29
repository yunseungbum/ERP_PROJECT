using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace BuddyErp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDuesExecutionAndYearSummaryV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "member_due_notes",
                columns: table => new
                {
                    member_due_note_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    member_id = table.Column<long>(type: "bigint", nullable: false),
                    due_year = table.Column<int>(type: "int", nullable: false),
                    execution_amount = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false, defaultValue: 0m),
                    content = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_due_notes", x => x.member_due_note_id);
                    table.ForeignKey(
                        name: "FK_member_due_notes_members_member_id",
                        column: x => x.member_id,
                        principalTable: "members",
                        principalColumn: "member_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "dues_year_summaries",
                columns: table => new
                {
                    dues_year_summary_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    due_year = table.Column<int>(type: "int", nullable: false),
                    notes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dues_year_summaries", x => x.dues_year_summary_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_dues_year_summaries_due_year",
                table: "dues_year_summaries",
                column: "due_year",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_member_due_notes_member_id_due_year",
                table: "member_due_notes",
                columns: new[] { "member_id", "due_year" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dues_year_summaries");

            migrationBuilder.DropTable(
                name: "member_due_notes");
        }
    }
}
