using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BattleTanks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_rooms_status",
                table: "rooms",
                newName: "IX_rooms_Status");

            migrationBuilder.RenameIndex(
                name: "IX_room_users_userId",
                table: "room_users",
                newName: "IX_room_users_UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_rooms_Status",
                table: "rooms",
                newName: "IX_rooms_status");

            migrationBuilder.RenameIndex(
                name: "IX_room_users_UserId",
                table: "room_users",
                newName: "IX_room_users_userId");
        }
    }
}
