using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BattleTanks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_users_Score",
                table: "users",
                column: "Score");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_Score",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "users");
        }
    }
}
