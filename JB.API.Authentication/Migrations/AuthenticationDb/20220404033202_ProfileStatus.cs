using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Authentication.Migrations.AuthenticationDb
{
    public partial class ProfileStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfileStatus",
                schema: "Authentication",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileStatus",
                schema: "Authentication",
                table: "AspNetUsers");
        }
    }
}
