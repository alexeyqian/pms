﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PMS.Data;

namespace PMS.Migrations
{
    [DbContext(typeof(PMSDBContext))]
    partial class PMSDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PMS.Models.Bug", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("ActualHours")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("ApprovedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("CommentCount")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Developer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("EstimatedHours")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("FirstPullRequestCommentCount")
                        .HasColumnType("int");

                    b.Property<DateTime?>("FirstPullRequestCommentDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("FirstPullRequestCommitCount")
                        .HasColumnType("int");

                    b.Property<DateTime?>("FirstPullRequestDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("FirstPullRequestIterationCount")
                        .HasColumnType("int");

                    b.Property<string>("FirstPullRequestStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("FixedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("NO")
                        .HasColumnType("int");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Priority")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PullRequestCount")
                        .HasColumnType("int");

                    b.Property<int>("RejectedTimes")
                        .HasColumnType("int");

                    b.Property<string>("ResolvedReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ResovedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Severity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("StartedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StatusInVS")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("SyncedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Tags")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Team")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Bug");
                });

            modelBuilder.Entity("PMS.Models.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Genre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Movie");
                });
#pragma warning restore 612, 618
        }
    }
}
