using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JB.Job.Migrations
{
    public partial class apliationdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Attachments",
                schema: "Job",
                table: "Application",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Introdution",
                schema: "Job",
                table: "Application",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachments",
                schema: "Job",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "Introdution",
                schema: "Job",
                table: "Application");
        }
    }
}
