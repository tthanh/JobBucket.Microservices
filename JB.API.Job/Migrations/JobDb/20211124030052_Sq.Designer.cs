﻿// <auto-generated />
using System;
using JB.Job.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JB.Job.Migrations
{
    [DbContext(typeof(JobDbContext))]
    [Migration("20211124030052_Sq")]
    partial class Sq
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Job")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("CategoryModelJobModel", b =>
                {
                    b.Property<int>("CategoriesId")
                        .HasColumnType("integer");

                    b.Property<int>("JobsId")
                        .HasColumnType("integer");

                    b.HasKey("CategoriesId", "JobsId");

                    b.HasIndex("JobsId");

                    b.ToTable("CategoryModelJobModel");
                });

            modelBuilder.Entity("JB.API.Job.Models.Job.ApplicationModel", b =>
                {
                    b.Property<int>("JobId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("CVId")
                        .HasColumnType("integer");

                    b.Property<string>("CVPDFUrl")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("JobId", "UserId");

                    b.ToTable("Application");
                });

            modelBuilder.Entity("JB.API.Job.Models.Job.CategoryModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasIdentityOptions(100L, null, null, null, null, null)
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("JB.API.Job.Models.Job.InterestModel", b =>
                {
                    b.Property<int>("JobId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("JobId", "UserId");

                    b.ToTable("Interests");
                });

            modelBuilder.Entity("JB.API.Job.Models.Job.JobModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasIdentityOptions(100L, null, null, null, null, null)
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ActiveStatus")
                        .HasColumnType("integer");

                    b.Property<string[]>("Addresses")
                        .HasColumnType("text[]");

                    b.Property<string>("Benefits")
                        .HasColumnType("text");

                    b.Property<string[]>("Cities")
                        .HasColumnType("text[]");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Cultures")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("EmployerId")
                        .HasColumnType("integer");

                    b.Property<string>("Experiences")
                        .HasColumnType("text");

                    b.Property<DateTime>("ExpireDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("Gender")
                        .HasColumnType("integer");

                    b.Property<string[]>("ImageUrls")
                        .HasColumnType("text[]");

                    b.Property<bool?>("IsVisaSponsorship")
                        .HasColumnType("boolean");

                    b.Property<string>("JobForm")
                        .HasColumnType("text");

                    b.Property<int?>("MaxSalary")
                        .HasColumnType("integer");

                    b.Property<int?>("MinSalary")
                        .HasColumnType("integer");

                    b.Property<int?>("NumberEmployeesToApplied")
                        .HasColumnType("integer");

                    b.Property<string>("OptionalRequirements")
                        .HasColumnType("text");

                    b.Property<int>("OrganizationId")
                        .HasColumnType("integer");

                    b.Property<int?>("Priority")
                        .HasColumnType("integer");

                    b.Property<string>("Requirements")
                        .HasColumnType("text");

                    b.Property<string>("Responsibilities")
                        .HasColumnType("text");

                    b.Property<string>("SalaryCurrency")
                        .HasColumnType("text");

                    b.Property<string>("SalaryDuration")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("Views")
                        .HasColumnType("integer");

                    b.Property<string>("WhyJoinUs")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("JB.API.Job.Models.Job.PositionModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasIdentityOptions(100L, null, null, null, null, null)
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("JB.API.Job.Models.Job.SkillModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasIdentityOptions(100L, null, null, null, null, null)
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Skillls");
                });

            modelBuilder.Entity("JB.API.Job.Models.Job.TypeModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasIdentityOptions(100L, null, null, null, null, null)
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Types");
                });

            modelBuilder.Entity("JobModelPositionModel", b =>
                {
                    b.Property<int>("JobsId")
                        .HasColumnType("integer");

                    b.Property<int>("PositionsId")
                        .HasColumnType("integer");

                    b.HasKey("JobsId", "PositionsId");

                    b.HasIndex("PositionsId");

                    b.ToTable("JobModelPositionModel");
                });

            modelBuilder.Entity("JobModelSkillModel", b =>
                {
                    b.Property<int>("JobsId")
                        .HasColumnType("integer");

                    b.Property<int>("SkillsId")
                        .HasColumnType("integer");

                    b.HasKey("JobsId", "SkillsId");

                    b.HasIndex("SkillsId");

                    b.ToTable("JobModelSkillModel");
                });

            modelBuilder.Entity("JobModelTypeModel", b =>
                {
                    b.Property<int>("JobsId")
                        .HasColumnType("integer");

                    b.Property<int>("TypesId")
                        .HasColumnType("integer");

                    b.HasKey("JobsId", "TypesId");

                    b.HasIndex("TypesId");

                    b.ToTable("JobModelTypeModel");
                });

            modelBuilder.Entity("CategoryModelJobModel", b =>
                {
                    b.HasOne("JB.API.Job.Models.Job.CategoryModel", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JB.API.Job.Models.Job.JobModel", null)
                        .WithMany()
                        .HasForeignKey("JobsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("JB.API.Job.Models.Job.ApplicationModel", b =>
                {
                    b.HasOne("JB.API.Job.Models.Job.JobModel", "Job")
                        .WithMany("Applications")
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Job");
                });

            modelBuilder.Entity("JB.API.Job.Models.Job.InterestModel", b =>
                {
                    b.HasOne("JB.API.Job.Models.Job.JobModel", "Job")
                        .WithMany("Interests")
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Job");
                });

            modelBuilder.Entity("JobModelPositionModel", b =>
                {
                    b.HasOne("JB.API.Job.Models.Job.JobModel", null)
                        .WithMany()
                        .HasForeignKey("JobsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JB.API.Job.Models.Job.PositionModel", null)
                        .WithMany()
                        .HasForeignKey("PositionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("JobModelSkillModel", b =>
                {
                    b.HasOne("JB.API.Job.Models.Job.JobModel", null)
                        .WithMany()
                        .HasForeignKey("JobsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JB.API.Job.Models.Job.SkillModel", null)
                        .WithMany()
                        .HasForeignKey("SkillsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("JobModelTypeModel", b =>
                {
                    b.HasOne("JB.API.Job.Models.Job.JobModel", null)
                        .WithMany()
                        .HasForeignKey("JobsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JB.API.Job.Models.Job.TypeModel", null)
                        .WithMany()
                        .HasForeignKey("TypesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("JB.API.Job.Models.Job.JobModel", b =>
                {
                    b.Navigation("Applications");

                    b.Navigation("Interests");
                });
#pragma warning restore 612, 618
        }
    }
}
