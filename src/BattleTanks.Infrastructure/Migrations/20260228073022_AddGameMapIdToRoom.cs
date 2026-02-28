using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BattleTanks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGameMapIdToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GameMapId",
                table: "rooms",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_rooms_GameMapId",
                table: "rooms",
                column: "GameMapId");

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_game_maps_GameMapId",
                table: "rooms",
                column: "GameMapId",
                principalTable: "game_maps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rooms_game_maps_GameMapId",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "IX_rooms_GameMapId",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "GameMapId",
                table: "rooms");
        }
    }
}
