﻿// <auto-generated />
using System;
using CloudWeather.Temparature.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CloudWeather.Temparature.Migrations
{
    [DbContext(typeof(TemparatureDbContext))]
    partial class TemparatureDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.33")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CloudWeather.Temparature.DataAccess.Temparature", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("TempHighF")
                        .HasColumnType("numeric");

                    b.Property<decimal>("TempLowF")
                        .HasColumnType("numeric");

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("temparature", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
