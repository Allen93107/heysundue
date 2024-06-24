﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Heysundue.Migrations.Article
{
    [DbContext(typeof(ArticleContext))]
    [Migration("20240619074552_UpdateArticleMode12")]
    partial class UpdateArticleMode12
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("Heysundue.Models.Accessdoor", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Room")
                        .HasColumnType("TEXT");

                    b.Property<string>("SearchColumn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Session")
                        .HasColumnType("TEXT");

                    b.Property<string>("StartDate")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Accessdoor");
                });

            modelBuilder.Entity("Heysundue.Models.Article", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Count")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Gender")
                        .HasColumnType("TEXT");

                    b.Property<string>("Location")
                        .HasColumnType("TEXT");

                    b.Property<string>("Number")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("Time")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Article");
                });

            modelBuilder.Entity("Heysundue.Models.Doorsystem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Room")
                        .HasColumnType("TEXT");

                    b.Property<string>("Session")
                        .HasColumnType("TEXT");

                    b.Property<string>("SessionName")
                        .HasColumnType("TEXT");

                    b.Property<string>("TimeRange")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Doorsystem");
                });

            modelBuilder.Entity("Heysundue.Models.Joinlist", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ChineseName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Country")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .HasColumnType("TEXT");

                    b.Property<string>("RegNo")
                        .HasColumnType("TEXT");

                    b.Property<string>("RegistrationStatus")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Joinlist");
                });

            modelBuilder.Entity("Heysundue.Models.Login", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Login");
                });

            modelBuilder.Entity("Heysundue.Models.Person", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Age")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Gender")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("Heysundue.Models.Registration", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DisplayLocation")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayStatus")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemName")
                        .HasColumnType("TEXT");

                    b.Property<int?>("TotalAmount")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TotalAmountUSD")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Registration");
                });
#pragma warning restore 612, 618
        }
    }
}
