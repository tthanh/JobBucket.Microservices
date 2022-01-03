using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Organization.Migrations.OrganizationDb
{
    public partial class updateAddresses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                schema: "Organization",
                table: "Organizations",
                newName: "Addresses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Addresses",
                schema: "Organization",
                table: "Organizations",
                newName: "Address");
        }
    }
}
