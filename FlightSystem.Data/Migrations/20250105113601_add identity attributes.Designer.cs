﻿// <auto-generated />
using System;
using FlightSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FlightSystem.Data.Migrations
{
    [DbContext(typeof(FlightContext))]
    [Migration("20250105113601_add identity attributes")]
    partial class addidentityattributes
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("FlightSystem.Data.Airport", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("Airports");
                });

            modelBuilder.Entity("FlightSystem.Data.Booking", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("PriceId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PriceId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("FlightSystem.Data.Currency", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Currency");
                });

            modelBuilder.Entity("FlightSystem.Data.Price", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Total")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.ToTable("Price");
                });

            modelBuilder.Entity("FlightSystem.Data.Segment", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("BookingId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Departure")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("DestinationId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OriginId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("DestinationId");

                    b.HasIndex("OriginId");

                    b.ToTable("Segments");
                });

            modelBuilder.Entity("FlightSystem.Data.Booking", b =>
                {
                    b.HasOne("FlightSystem.Data.Price", "Price")
                        .WithMany()
                        .HasForeignKey("PriceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Price");
                });

            modelBuilder.Entity("FlightSystem.Data.Price", b =>
                {
                    b.HasOne("FlightSystem.Data.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");
                });

            modelBuilder.Entity("FlightSystem.Data.Segment", b =>
                {
                    b.HasOne("FlightSystem.Data.Booking", null)
                        .WithMany("Segments")
                        .HasForeignKey("BookingId");

                    b.HasOne("FlightSystem.Data.Airport", "Destination")
                        .WithMany()
                        .HasForeignKey("DestinationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FlightSystem.Data.Airport", "Origin")
                        .WithMany()
                        .HasForeignKey("OriginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Destination");

                    b.Navigation("Origin");
                });

            modelBuilder.Entity("FlightSystem.Data.Booking", b =>
                {
                    b.Navigation("Segments");
                });
#pragma warning restore 612, 618
        }
    }
}