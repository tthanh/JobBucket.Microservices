using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Job.Migrations
{
    public partial class RemoveIntGender : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "Job",
                table: "Jobs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Gender",
                schema: "Job",
                table: "Jobs",
                type: "integer",
                nullable: true);
        }
    }
}
