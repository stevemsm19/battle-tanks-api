using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BattleTanks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGameMapsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game_maps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Layout = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_maps", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "game_maps",
                columns: new[] { "Id", "Layout", "Name" },
                values: new object[] { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), "[[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],[0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0],[0,0,2,2,0,1,0,1,0,2,2,2,2,2,2,0,1,0,1,0,2,2,0,0],[0,0,2,2,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,2,2,0,0],[0,0,0,0,0,1,1,1,0,1,1,0,0,1,1,0,1,1,1,0,0,0,0,0],[0,1,1,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0],[0,0,0,0,2,2,0,0,0,1,0,1,1,0,1,0,0,0,2,2,0,0,0,0],[0,0,1,0,2,2,0,1,0,0,0,0,0,0,0,0,1,0,2,2,0,1,0,0],[0,0,1,0,0,0,0,1,1,1,0,2,2,0,1,1,1,0,0,0,0,1,0,0],[0,0,0,0,2,2,0,0,0,0,0,2,2,0,0,0,0,0,2,2,0,0,0,0],[0,1,1,0,2,2,0,1,1,1,0,0,0,0,1,1,1,0,2,2,0,1,1,0],[0,0,0,0,0,0,0,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,0],[0,0,2,2,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1,0,2,2,0,0],[0,0,2,2,0,0,0,1,0,2,2,2,2,2,2,0,1,0,0,0,2,2,0,0],[0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0],[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]]", "Default Map" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_maps");
        }
    }
}
