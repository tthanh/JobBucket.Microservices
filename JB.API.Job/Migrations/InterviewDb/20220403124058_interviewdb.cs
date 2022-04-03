using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.API.Migrations.InterviewDb
{
    public partial class interviewdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Form",
                schema: "Interview",
                table: "Interviews",
                newName: "Forms");

            migrationBuilder.AddColumn<int>(
                name: "CurrentInterviewRound",
                schema: "Interview",
                table: "Interviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalInterviewRound",
                schema: "Interview",
                table: "Interviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentInterviewRound",
                schema: "Interview",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "TotalInterviewRound",
                schema: "Interview",
                table: "Interviews");

            migrationBuilder.RenameColumn(
                name: "Forms",
                schema: "Interview",
                table: "Interviews",
                newName: "Form");
        }
    }
}
