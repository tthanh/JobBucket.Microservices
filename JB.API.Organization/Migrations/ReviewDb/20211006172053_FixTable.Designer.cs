﻿// <auto-generated />
using System;
using JB.Organization.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JB.Organization.Migrations.ReviewDb
{
    [DbContext(typeof(ReviewDbContext))]
    [Migration("20211006172053_FixTable")]
    partial class FixTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Review")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("JB.Organization.Models.Review.ReviewInterestModel", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("ReviewId")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "ReviewId");

                    b.HasIndex("ReviewId");

                    b.ToTable("ReviewInterests");
                });

            modelBuilder.Entity("JB.Organization.Models.Review.ReviewModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("OrganizationId")
                        .HasColumnType("integer");

                    b.Property<float>("Rating")
                        .HasColumnType("real");

                    b.Property<int>("RatingBenefit")
                        .HasColumnType("integer");

                    b.Property<int>("RatingCulture")
                        .HasColumnType("integer");

                    b.Property<int>("RatingLearning")
                        .HasColumnType("integer");

                    b.Property<int>("RatingWorkspace")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("JB.Organization.Models.Review.ReviewInterestModel", b =>
                {
                    b.HasOne("JB.Organization.Models.Review.ReviewModel", "Review")
                        .WithMany("Interests")
                        .HasForeignKey("ReviewId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Review");
                });

            modelBuilder.Entity("JB.Organization.Models.Review.ReviewModel", b =>
                {
                    b.Navigation("Interests");
                });
#pragma warning restore 612, 618
        }
    }
}
