using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.User.Migrations.ProfileDb
{
    public partial class RemoveUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Profile",
                table: "Profiles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "Profile",
                table: "Profiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
