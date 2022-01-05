using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Authentication.Migrations.AuthenticationDb
{
    public partial class cvuserinit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultCVId",
                schema: "Authentication",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultCVId",
                schema: "Authentication",
                table: "AspNetUsers");
        }
    }
}
