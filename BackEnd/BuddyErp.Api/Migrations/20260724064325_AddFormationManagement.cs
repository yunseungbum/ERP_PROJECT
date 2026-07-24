using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace BuddyErp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFormationManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "match_schedules",
                columns: table => new
                {
                    schedule_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    venue_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    opponent_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    starts_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_schedules", x => x.schedule_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "match_participants",
                columns: table => new
                {
                    participant_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    schedule_id = table.Column<long>(type: "bigint", nullable: false),
                    member_id = table.Column<long>(type: "bigint", nullable: true),
                    guest_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    is_guest = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_participants", x => x.participant_id);
                    table.ForeignKey(
                        name: "FK_match_participants_match_schedules_schedule_id",
                        column: x => x.schedule_id,
                        principalTable: "match_schedules",
                        principalColumn: "schedule_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_match_participants_members_member_id",
                        column: x => x.member_id,
                        principalTable: "members",
                        principalColumn: "member_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "quarter_formations",
                columns: table => new
                {
                    quarter_formation_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    schedule_id = table.Column<long>(type: "bigint", nullable: false),
                    quarter_number = table.Column<int>(type: "int", nullable: false),
                    formation_code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quarter_formations", x => x.quarter_formation_id);
                    table.ForeignKey(
                        name: "FK_quarter_formations_match_schedules_schedule_id",
                        column: x => x.schedule_id,
                        principalTable: "match_schedules",
                        principalColumn: "schedule_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "quarter_lineup_players",
                columns: table => new
                {
                    lineup_player_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    quarter_formation_id = table.Column<long>(type: "bigint", nullable: false),
                    participant_id = table.Column<long>(type: "bigint", nullable: false),
                    slot_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    position_order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quarter_lineup_players", x => x.lineup_player_id);
                    table.ForeignKey(
                        name: "FK_quarter_lineup_players_match_participants_participant_id",
                        column: x => x.participant_id,
                        principalTable: "match_participants",
                        principalColumn: "participant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quarter_lineup_players_quarter_formations_quarter_formation_~",
                        column: x => x.quarter_formation_id,
                        principalTable: "quarter_formations",
                        principalColumn: "quarter_formation_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "match_schedules",
                columns: new[] { "schedule_id", "created_at", "opponent_name", "starts_at", "updated_at", "venue_name" },
                values: new object[] { 1L, new DateTime(2026, 7, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "신풍 FC", new DateTime(2026, 8, 20, 20, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 7, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "신트리 공원" });

            migrationBuilder.CreateIndex(
                name: "IX_match_participants_member_id",
                table: "match_participants",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_participants_schedule_id_guest_name",
                table: "match_participants",
                columns: new[] { "schedule_id", "guest_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_match_participants_schedule_id_member_id",
                table: "match_participants",
                columns: new[] { "schedule_id", "member_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quarter_formations_schedule_id_quarter_number",
                table: "quarter_formations",
                columns: new[] { "schedule_id", "quarter_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quarter_lineup_players_participant_id",
                table: "quarter_lineup_players",
                column: "participant_id");

            migrationBuilder.CreateIndex(
                name: "IX_quarter_lineup_players_quarter_formation_id_participant_id",
                table: "quarter_lineup_players",
                columns: new[] { "quarter_formation_id", "participant_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quarter_lineup_players_quarter_formation_id_slot_code",
                table: "quarter_lineup_players",
                columns: new[] { "quarter_formation_id", "slot_code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quarter_lineup_players");

            migrationBuilder.DropTable(
                name: "match_participants");

            migrationBuilder.DropTable(
                name: "quarter_formations");

            migrationBuilder.DropTable(
                name: "match_schedules");
        }
    }
}
