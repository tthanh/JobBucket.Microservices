using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Job.Migrations.InterviewDb
{
    public partial class cvid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "interview");

            migrationBuilder.RenameTable(
                name: "Interviews",
                schema: "Interview",
                newName: "Interviews",
                newSchema: "interview");

            migrationBuilder.AddColumn<int>(
                name: "IntervieweeCVId",
                schema: "interview",
                table: "Interviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                schema: "interview",
                table: "Interviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntervieweeCVId",
                schema: "interview",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                schema: "interview",
                table: "Interviews");

            migrationBuilder.EnsureSchema(
                name: "Interview");

            migrationBuilder.RenameTable(
                name: "Interviews",
                schema: "interview",
                newName: "Interviews",
                newSchema: "Interview");
        }
    }
}
