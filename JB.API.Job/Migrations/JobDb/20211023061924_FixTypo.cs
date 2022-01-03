using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Job.Migrations
{
    public partial class FixTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                schema: "Job",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "IsVisaSponsorhip",
                schema: "Job",
                table: "Jobs",
                newName: "IsVisaSponsorship");

            migrationBuilder.AddColumn<string[]>(
                name: "Addresses",
                schema: "Job",
                table: "Jobs",
                type: "text[]",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Addresses",
                schema: "Job",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "IsVisaSponsorship",
                schema: "Job",
                table: "Jobs",
                newName: "IsVisaSponsorhip");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "Job",
                table: "Jobs",
                type: "text",
                nullable: true);
        }
    }
}
