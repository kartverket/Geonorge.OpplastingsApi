﻿// <auto-generated />
using System;
using Geonorge.OpplastingsApi.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Geonorge.OpplastingsApi.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20230706080320_AddContactEmailExtra")]
    partial class AddContactEmailExtra
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DatasetAllowedFileFormats", b =>
                {
                    b.Property<string>("AllowedFileFormatsExtension")
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("DatasetsId")
                        .HasColumnType("int");

                    b.HasKey("AllowedFileFormatsExtension", "DatasetsId");

                    b.HasIndex("DatasetsId");

                    b.ToTable("DatasetAllowedFileFormats");
                });

            modelBuilder.Entity("Geonorge.OpplastingsApi.Models.Entity.Dataset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ContactEmail")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ContactEmailExtra")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ContactName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("MetadataUuid")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("OwnerOrganization")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("RequireValidFile")
                        .HasColumnType("bit");

                    b.Property<string>("RequiredRole")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Datasets");
                });

            modelBuilder.Entity("Geonorge.OpplastingsApi.Models.Entity.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DatasetId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UploaderEmail")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UploaderOrganization")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UploaderPerson")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UploaderUsername")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("DatasetId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Geonorge.OpplastingsApi.Models.Entity.FileFormat", b =>
                {
                    b.Property<string>("Extension")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Extension");

                    b.ToTable("FileFormats");

                    b.HasData(
                        new
                        {
                            Extension = "gml",
                            Name = "Geography Markup Language"
                        },
                        new
                        {
                            Extension = "sos",
                            Name = "Samordnet Opplegg for Stedfestet Informasjon (SOSI)"
                        },
                        new
                        {
                            Extension = "gdb",
                            Name = "ESRI file Geodatabase"
                        },
                        new
                        {
                            Extension = "shp",
                            Name = "Shape"
                        },
                        new
                        {
                            Extension = "gpkg",
                            Name = "GeoPackage"
                        });
                });

            modelBuilder.Entity("DatasetAllowedFileFormats", b =>
                {
                    b.HasOne("Geonorge.OpplastingsApi.Models.Entity.FileFormat", null)
                        .WithMany()
                        .HasForeignKey("AllowedFileFormatsExtension")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Geonorge.OpplastingsApi.Models.Entity.Dataset", null)
                        .WithMany()
                        .HasForeignKey("DatasetsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Geonorge.OpplastingsApi.Models.Entity.File", b =>
                {
                    b.HasOne("Geonorge.OpplastingsApi.Models.Entity.Dataset", "Dataset")
                        .WithMany("Files")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Dataset");
                });

            modelBuilder.Entity("Geonorge.OpplastingsApi.Models.Entity.Dataset", b =>
                {
                    b.Navigation("Files");
                });
#pragma warning restore 612, 618
        }
    }
}