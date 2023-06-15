using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameApp.Domain.Migrations
{
    public partial class AddRoomManager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ManagerId",
                table: "Rooms",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Users_ManagerId",
                table: "Rooms",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Users_ManagerId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_ManagerId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Rooms");
        }
    }
}
