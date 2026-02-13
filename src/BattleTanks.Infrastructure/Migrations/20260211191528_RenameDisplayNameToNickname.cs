using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BattleTanks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameDisplayNameToNickname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "users",
                newName: "Nickname");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nickname",
                table: "users",
                newName: "DisplayName");
        }
    }
}
