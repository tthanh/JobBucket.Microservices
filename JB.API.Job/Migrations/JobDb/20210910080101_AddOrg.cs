using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JB.Job.Migrations
{
    public partial class AddOrg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Job");

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "Job",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                schema: "Job",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true),
                    ImageUrls = table.Column<string[]>(type: "text[]", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ActiveStatus = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Cities = table.Column<string[]>(type: "text[]", nullable: true),
                    MinSalary = table.Column<int>(type: "integer", nullable: false),
                    MaxSalary = table.Column<int>(type: "integer", nullable: false),
                    SalaryCurrency = table.Column<string>(type: "text", nullable: true),
                    SalaryDuration = table.Column<string>(type: "text", nullable: true),
                    IsVisaSponsorhip = table.Column<bool>(type: "boolean", nullable: false),
                    EmployerId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Benefits = table.Column<string>(type: "text", nullable: true),
                    Experiences = table.Column<string>(type: "text", nullable: true),
                    Responsibilities = table.Column<string>(type: "text", nullable: true),
                    Requirements = table.Column<string>(type: "text", nullable: true),
                    OptionalRequirements = table.Column<string>(type: "text", nullable: true),
                    Cultures = table.Column<string>(type: "text", nullable: true),
                    WhyJoinUs = table.Column<string>(type: "text", nullable: true),
                    NumberEmployeesToApplied = table.Column<int>(type: "integer", nullable: false),
                    JobForm = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    Views = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                schema: "Job",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skillls",
                schema: "Job",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skillls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Types",
                schema: "Job",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Application",
                schema: "Job",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => new { x.JobId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Application_Jobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "Job",
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryModelJobModel",
                schema: "Job",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "integer", nullable: false),
                    JobsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryModelJobModel", x => new { x.CategoriesId, x.JobsId });
                    table.ForeignKey(
                        name: "FK_CategoryModelJobModel_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalSchema: "Job",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryModelJobModel_Jobs_JobsId",
                        column: x => x.JobsId,
                        principalSchema: "Job",
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Interests",
                schema: "Job",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    JobId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interests", x => new { x.JobId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Interests_Jobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "Job",
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobModelPositionModel",
                schema: "Job",
                columns: table => new
                {
                    JobsId = table.Column<int>(type: "integer", nullable: false),
                    PositionsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobModelPositionModel", x => new { x.JobsId, x.PositionsId });
                    table.ForeignKey(
                        name: "FK_JobModelPositionModel_Jobs_JobsId",
                        column: x => x.JobsId,
                        principalSchema: "Job",
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobModelPositionModel_Positions_PositionsId",
                        column: x => x.PositionsId,
                        principalSchema: "Job",
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobModelSkillModel",
                schema: "Job",
                columns: table => new
                {
                    JobsId = table.Column<int>(type: "integer", nullable: false),
                    SkillsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobModelSkillModel", x => new { x.JobsId, x.SkillsId });
                    table.ForeignKey(
                        name: "FK_JobModelSkillModel_Jobs_JobsId",
                        column: x => x.JobsId,
                        principalSchema: "Job",
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobModelSkillModel_Skillls_SkillsId",
                        column: x => x.SkillsId,
                        principalSchema: "Job",
                        principalTable: "Skillls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobModelTypeModel",
                schema: "Job",
                columns: table => new
                {
                    JobsId = table.Column<int>(type: "integer", nullable: false),
                    TypesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobModelTypeModel", x => new { x.JobsId, x.TypesId });
                    table.ForeignKey(
                        name: "FK_JobModelTypeModel_Jobs_JobsId",
                        column: x => x.JobsId,
                        principalSchema: "Job",
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobModelTypeModel_Types_TypesId",
                        column: x => x.TypesId,
                        principalSchema: "Job",
                        principalTable: "Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryModelJobModel_JobsId",
                schema: "Job",
                table: "CategoryModelJobModel",
                column: "JobsId");

            migrationBuilder.CreateIndex(
                name: "IX_JobModelPositionModel_PositionsId",
                schema: "Job",
                table: "JobModelPositionModel",
                column: "PositionsId");

            migrationBuilder.CreateIndex(
                name: "IX_JobModelSkillModel_SkillsId",
                schema: "Job",
                table: "JobModelSkillModel",
                column: "SkillsId");

            migrationBuilder.CreateIndex(
                name: "IX_JobModelTypeModel_TypesId",
                schema: "Job",
                table: "JobModelTypeModel",
                column: "TypesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Application",
                schema: "Job");

            migrationBuilder.DropTable(
                name: "CategoryModelJobModel",
                schema: "Job");

            migrationBuilder.DropTable(
                name: "Interests",
                schema: "Job");

            migrationBuilder.DropTable(
                name: "JobModelPositionModel",
                schema: "Job");

            migrationBuilder.DropTable(
                name: "JobModelSkillModel",
                schema: "Job");

            migrationBuilder.DropTable(
                name: "JobModelTypeModel",
                schema: "Job");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "Job");

            migrationBuilder.DropTable(
                name: "Positions",
                schema: "Job");

            migrationBuilder.DropTable(
                name: "Skillls",
                schema: "Job");

            migrationBuilder.DropTable(
                name: "Jobs",
                schema: "Job");

            migrationBuilder.DropTable(
                name: "Types",
                schema: "Job");
        }
    }
}
