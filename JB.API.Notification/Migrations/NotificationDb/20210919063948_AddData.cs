using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Notification.Migrations.NotificationDb
{
    public partial class AddData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "_data",
                schema: "Notification",
                table: "Notifications",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_data",
                schema: "Notification",
                table: "Notifications");
        }
    }
}
