using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.User.Migrations.ProfileDb
{
    public partial class AddCity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "Profile",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "Profile",
                table: "Profiles",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                schema: "Profile",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Country",
                schema: "Profile",
                table: "Profiles");
        }
    }
}
