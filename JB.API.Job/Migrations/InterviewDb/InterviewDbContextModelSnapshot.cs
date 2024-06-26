﻿// <auto-generated />
using System;
using System.Collections.Generic;
using JB.Job.Data;
using JB.Job.Models.Interview;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JB.API.Migrations.InterviewDb
{
    [DbContext(typeof(InterviewDbContext))]
    partial class InterviewDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Interview")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("JB.Job.Models.Interview.InterviewModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("CurrentInterviewRound")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<ICollection<InterviewFormModel>>("Forms")
                        .HasColumnType("jsonb");

                    b.Property<DateTime>("InterviewTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("IntervieweeCVId")
                        .HasColumnType("integer");

                    b.Property<int>("IntervieweeId")
                        .HasColumnType("integer");

                    b.Property<int>("InterviewerId")
                        .HasColumnType("integer");

                    b.Property<int>("JobId")
                        .HasColumnType("integer");

                    b.Property<int>("OrganizationId")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("TotalInterviewRound")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Interviews");
                });
#pragma warning restore 612, 618
        }
    }
}
