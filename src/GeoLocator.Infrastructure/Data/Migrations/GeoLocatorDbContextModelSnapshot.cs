﻿// <auto-generated />
using System;
using GeoLocator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GeoLocator.Infrastructure.Data.Migrations
{
    [DbContext(typeof(GeoLocatorDbContext))]
    partial class GeoLocatorDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("GeoLocator.Core.Entities.LocationAggregate.IpAddress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("LocationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("IpAddress");
                });

            modelBuilder.Entity("GeoLocator.Core.Entities.LocationAggregate.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CountryCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("GeoLocator.Core.Entities.LocationAggregate.IpAddress", b =>
                {
                    b.HasOne("GeoLocator.Core.Entities.LocationAggregate.Location", null)
                        .WithMany("IpAddresses")
                        .HasForeignKey("LocationId");
                });

            modelBuilder.Entity("GeoLocator.Core.Entities.LocationAggregate.Location", b =>
                {
                    b.Navigation("IpAddresses");
                });
#pragma warning restore 612, 618
        }
    }
}
