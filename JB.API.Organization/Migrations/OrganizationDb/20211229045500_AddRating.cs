using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Organization.Migrations.OrganizationDb
{
    public partial class AddRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Rating",
                schema: "Organization",
                table: "Organizations",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "RatingBenefit",
                schema: "Organization",
                table: "Organizations",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "RatingCulture",
                schema: "Organization",
                table: "Organizations",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "RatingLearning",
                schema: "Organization",
                table: "Organizations",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "RatingWorkspace",
                schema: "Organization",
                table: "Organizations",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                schema: "Organization",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "RatingBenefit",
                schema: "Organization",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "RatingCulture",
                schema: "Organization",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "RatingLearning",
                schema: "Organization",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "RatingWorkspace",
                schema: "Organization",
                table: "Organizations");
        }
    }
}
