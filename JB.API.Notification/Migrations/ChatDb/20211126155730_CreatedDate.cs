using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Notification.Migrations.ChatDb
{
    public partial class CreatedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "Chat",
                table: "Conversations",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                schema: "Chat",
                table: "Conversations",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "Chat",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                schema: "Chat",
                table: "Conversations");
        }
    }
}
