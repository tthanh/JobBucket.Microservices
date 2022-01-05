using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.User.Migrations.CVDb
{
    public partial class AddCity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "CV",
                table: "CVs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "CV",
                table: "CVs",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                schema: "CV",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "Country",
                schema: "CV",
                table: "CVs");
        }
    }
}
