using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameApp.Domain.Migrations
{
    public partial class M2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Logig",
                table: "Users",
                newName: "Login");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Login",
                table: "Users",
                newName: "Logig");
        }
    }
}
