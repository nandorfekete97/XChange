﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using XChange.Data.context;

#nullable disable

namespace XChange.Migrations
{
    [DbContext(typeof(XChangeContext))]
    partial class XChangeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("XChange.Data.Entities.BookKeepingEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("ExchangeInfoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("BookKeepings");
                });

            modelBuilder.Entity("XChange.Data.Entities.CompanyExchangeFundEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("CompanyExchangeFunds");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Balance = 500000m
                        });
                });

            modelBuilder.Entity("XChange.Data.Entities.CurrencyEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Currencies");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Forint",
                            ShortName = "HUF"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Dollar",
                            ShortName = "USD"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Euro",
                            ShortName = "EUR"
                        });
                });

            modelBuilder.Entity("XChange.Data.Entities.CurrencyRateEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CurrencyId")
                        .HasColumnType("int");

                    b.Property<decimal>("Rate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("CurrencyRates");
                });

            modelBuilder.Entity("XChange.Data.Entities.ExchangeInfoEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CurrencyRateId")
                        .HasColumnType("int");

                    b.Property<int?>("Error")
                        .HasColumnType("int");

                    b.Property<DateTime?>("FailedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FinalizedAt")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("SourceCurrencyAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("SourceCurrencyId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("TargetCurrencyId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ExchangeInfos");
                });

            modelBuilder.Entity("XChange.Data.Entities.UserEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            FirstName = "Kobe",
                            LastName = "Bryant"
                        },
                        new
                        {
                            Id = 2,
                            FirstName = "Jason",
                            LastName = "Kidd"
                        },
                        new
                        {
                            Id = 3,
                            FirstName = "Lebron",
                            LastName = "James"
                        });
                });

            modelBuilder.Entity("XChange.Data.Entities.UserFundEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CurrencyId")
                        .HasColumnType("int");

                    b.Property<decimal>("Disposable")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Pending")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("UserFunds");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CurrencyId = 3,
                            Disposable = 500m,
                            Pending = 0m,
                            UserId = 1
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
