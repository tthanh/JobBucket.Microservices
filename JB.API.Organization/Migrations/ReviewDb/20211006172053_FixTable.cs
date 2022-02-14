using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Organization.Migrations.ReviewDb
{
    public partial class FixTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewInterestModel_Reviews_ReviewId",
                schema: "Review",
                table: "ReviewInterestModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewInterestModel",
                schema: "Review",
                table: "ReviewInterestModel");

            migrationBuilder.RenameTable(
                name: "ReviewInterestModel",
                schema: "Review",
                newName: "ReviewInterests",
                newSchema: "Review");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewInterestModel_ReviewId",
                schema: "Review",
                table: "ReviewInterests",
                newName: "IX_ReviewInterests_ReviewId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewInterests",
                schema: "Review",
                table: "ReviewInterests",
                columns: new[] { "UserId", "ReviewId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewInterests_Reviews_ReviewId",
                schema: "Review",
                table: "ReviewInterests",
                column: "ReviewId",
                principalSchema: "Review",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewInterests_Reviews_ReviewId",
                schema: "Review",
                table: "ReviewInterests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewInterests",
                schema: "Review",
                table: "ReviewInterests");

            migrationBuilder.RenameTable(
                name: "ReviewInterests",
                schema: "Review",
                newName: "ReviewInterestModel",
                newSchema: "Review");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewInterests_ReviewId",
                schema: "Review",
                table: "ReviewInterestModel",
                newName: "IX_ReviewInterestModel_ReviewId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewInterestModel",
                schema: "Review",
                table: "ReviewInterestModel",
                columns: new[] { "UserId", "ReviewId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewInterestModel_Reviews_ReviewId",
                schema: "Review",
                table: "ReviewInterestModel",
                column: "ReviewId",
                principalSchema: "Review",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
