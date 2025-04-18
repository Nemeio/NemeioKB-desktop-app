﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nemeio.Infrastructure;

namespace Nemeio.Infrastructure.Migrations
{
    [DbContext(typeof(NemeioDbContext))]
    [Migration("20211108142851_AddUpdateToOnSettings")]
    partial class AddUpdateToOnSettings
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("Nemeio.Infrastructure.DbModels.ApplicationSettingsDbModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AugmentedImageEnable");

                    b.Property<string>("Language");

                    b.Property<bool>("ShowGrantPrivilegeWindow");

                    b.Property<string>("UpdateTo");

                    b.HasKey("Id");

                    b.ToTable("ApplicationSettings");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AugmentedImageEnable = true,
                            ShowGrantPrivilegeWindow = true
                        });
                });

            modelBuilder.Entity("Nemeio.Infrastructure.DbModels.BlacklistDbModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsSystem");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Blacklists");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IsSystem = true,
                            Name = "nemeio"
                        },
                        new
                        {
                            Id = 2,
                            IsSystem = true,
                            Name = "explorer"
                        });
                });

            modelBuilder.Entity("Nemeio.Infrastructure.DbModels.CategoryDbModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ConfiguratorIndex");

                    b.Property<bool>("IsDefault");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConfiguratorIndex = 0,
                            IsDefault = true,
                            Title = "Default"
                        });
                });

            modelBuilder.Entity("Nemeio.Infrastructure.DbModels.LayoutDbModel", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("AssociatedId");

                    b.Property<bool>("AugmentedImageEnabled");

                    b.Property<int>("CategoryId");

                    b.Property<int>("ConfiguratorIndex");

                    b.Property<DateTime>("DateCreation");

                    b.Property<DateTime>("DateUpdate");

                    b.Property<bool>("Enable");

                    b.Property<bool>("FontIsBold");

                    b.Property<bool>("FontIsItalic");

                    b.Property<string>("FontName");

                    b.Property<int>("FontSize");

                    b.Property<byte[]>("Image")
                        .IsRequired();

                    b.Property<int>("ImageType");

                    b.Property<bool>("IsDarkMode");

                    b.Property<bool>("IsDefault");

                    b.Property<bool>("IsFactory");

                    b.Property<bool>("IsHid");

                    b.Property<bool>("IsTemplate");

                    b.Property<string>("Keys");

                    b.Property<bool>("LinkAppEnable");

                    b.Property<string>("LinkAppPath");

                    b.Property<string>("OsId")
                        .IsRequired();

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Layouts");
                });

            modelBuilder.Entity("Nemeio.Infrastructure.DbModels.LayoutDbModel", b =>
                {
                    b.HasOne("Nemeio.Infrastructure.DbModels.CategoryDbModel", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
