using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameApp.Domain.Migrations
{
    public partial class NullableWinnerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Users_WinnerId",
                table: "Games");

            migrationBuilder.AlterColumn<int>(
                name: "WinnerId",
                table: "Games",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Users_WinnerId",
                table: "Games",
                column: "WinnerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Users_WinnerId",
                table: "Games");

            migrationBuilder.AlterColumn<int>(
                name: "WinnerId",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Users_WinnerId",
                table: "Games",
                column: "WinnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
