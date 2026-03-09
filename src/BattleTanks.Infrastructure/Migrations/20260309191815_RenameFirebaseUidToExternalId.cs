using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BattleTanks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameFirebaseUidToExternalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirebaseUid",
                table: "users",
                newName: "ExternalId");

            migrationBuilder.RenameIndex(
                name: "IX_users_FirebaseUid",
                table: "users",
                newName: "IX_users_ExternalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExternalId",
                table: "users",
                newName: "FirebaseUid");

            migrationBuilder.RenameIndex(
                name: "IX_users_ExternalId",
                table: "users",
                newName: "IX_users_FirebaseUid");
        }
    }
}
