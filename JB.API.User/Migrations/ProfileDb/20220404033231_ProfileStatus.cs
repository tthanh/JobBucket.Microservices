using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.User.Migrations.ProfileDb
{
    public partial class ProfileStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfileStatus",
                schema: "Profile",
                table: "Profiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileStatus",
                schema: "Profile",
                table: "Profiles");
        }
    }
}
