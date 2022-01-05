using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.User.Migrations.CVDb
{
    public partial class AddCVName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CVName",
                schema: "CV",
                table: "CVs",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CVName",
                schema: "CV",
                table: "CVs");
        }
    }
}
