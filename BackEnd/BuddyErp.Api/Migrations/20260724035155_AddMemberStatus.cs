using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuddyErp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "member_status",
                table: "members",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "member_status",
                table: "members");
        }
    }
}
