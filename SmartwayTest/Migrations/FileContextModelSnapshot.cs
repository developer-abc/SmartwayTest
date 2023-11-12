﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartwayTest.Data;

#nullable disable

namespace SmartwayTest.Migrations
{
    [DbContext(typeof(FileContext))]
    partial class FileContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SmartwayTest.Model.FileModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DownloadToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("FileContent")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GroupModelId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Progress")
                        .HasColumnType("float");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GroupModelId");

                    b.HasIndex("UserId");

                    b.ToTable("Files", (string)null);
                });

            modelBuilder.Entity("SmartwayTest.Model.GroupModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DownloadToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Groups", (string)null);
                });

            modelBuilder.Entity("SmartwayTest.Model.FileModel", b =>
                {
                    b.HasOne("SmartwayTest.Model.GroupModel", "GroupModel")
                        .WithMany("Files")
                        .HasForeignKey("GroupModelId");

                    b.Navigation("GroupModel");
                });

            modelBuilder.Entity("SmartwayTest.Model.GroupModel", b =>
                {
                    b.Navigation("Files");
                });
#pragma warning restore 612, 618
        }
    }
}
