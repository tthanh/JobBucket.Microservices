using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Organization.Migrations.ReviewDb
{
    public partial class FixTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewInterestModel_Notifications_ReviewId",
                schema: "Review",
                table: "ReviewInterestModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                schema: "Review",
                table: "Notifications");

            migrationBuilder.RenameTable(
                name: "Notifications",
                schema: "Review",
                newName: "Reviews",
                newSchema: "Review");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                schema: "Review",
                table: "Reviews",
                column: "Id");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewInterestModel_Reviews_ReviewId",
                schema: "Review",
                table: "ReviewInterestModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                schema: "Review",
                table: "Reviews");

            migrationBuilder.RenameTable(
                name: "Reviews",
                schema: "Review",
                newName: "Notifications",
                newSchema: "Review");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                schema: "Review",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewInterestModel_Notifications_ReviewId",
                schema: "Review",
                table: "ReviewInterestModel",
                column: "ReviewId",
                principalSchema: "Review",
                principalTable: "Notifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
