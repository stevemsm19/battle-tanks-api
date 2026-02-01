using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BattleTanks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOpen",
                table: "rooms");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "rooms",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "rooms");

            migrationBuilder.AddColumn<bool>(
                name: "IsOpen",
                table: "rooms",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
