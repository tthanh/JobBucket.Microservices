using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Notification.Migrations.NotificationDb
{
    public partial class DataType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "_data",
                schema: "Notification",
                table: "Notifications",
                newName: "Data");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                schema: "Notification",
                table: "Notifications",
                newName: "_data");
        }
    }
}
