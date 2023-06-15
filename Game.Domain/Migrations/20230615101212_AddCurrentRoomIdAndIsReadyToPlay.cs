using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameApp.Domain.Migrations
{
    public partial class AddCurrentRoomIdAndIsReadyToPlay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentRoomId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isReadyToPlay",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentRoomId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "isReadyToPlay",
                table: "Users");
        }
    }
}
