using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JB.Organization.Migrations.ReviewDb
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Review");

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "Review",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rating = table.Column<float>(type: "real", nullable: false),
                    RatingBenefit = table.Column<int>(type: "integer", nullable: false),
                    RatingLearning = table.Column<int>(type: "integer", nullable: false),
                    RatingCulture = table.Column<int>(type: "integer", nullable: false),
                    RatingWorkspace = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewInterestModel",
                schema: "Review",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ReviewId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewInterestModel", x => new { x.UserId, x.ReviewId });
                    table.ForeignKey(
                        name: "FK_ReviewInterestModel_Notifications_ReviewId",
                        column: x => x.ReviewId,
                        principalSchema: "Review",
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewInterestModel_ReviewId",
                schema: "Review",
                table: "ReviewInterestModel",
                column: "ReviewId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewInterestModel",
                schema: "Review");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "Review");
        }
    }
}
