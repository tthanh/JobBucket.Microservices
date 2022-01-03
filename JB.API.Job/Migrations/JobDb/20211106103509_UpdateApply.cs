using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Job.Migrations
{
    public partial class UpdateApply : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CVId",
                schema: "Job",
                table: "Application",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CVPDFUrl",
                schema: "Job",
                table: "Application",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Job",
                table: "Application",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CVId",
                schema: "Job",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "CVPDFUrl",
                schema: "Job",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Job",
                table: "Application");
        }
    }
}
