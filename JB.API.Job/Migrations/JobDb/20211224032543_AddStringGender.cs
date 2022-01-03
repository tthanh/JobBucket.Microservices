using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Job.Migrations
{
    public partial class AddStringGender : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                schema: "Job",
                table: "Jobs",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "Job",
                table: "Jobs");
        }
    }
}
