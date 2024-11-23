﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Voicemail;

#nullable disable

namespace Voicemail.Migrations
{
    [DbContext(typeof(VoicemailContext))]
    partial class VoicemailContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("Voicemail.Models.Call", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ExternalId")
                        .HasColumnType("TEXT");

                    b.Property<string>("NumberForwardedFrom")
                        .HasColumnType("TEXT");

                    b.Property<string>("NumberFrom")
                        .HasColumnType("TEXT");

                    b.Property<string>("NumberTo")
                        .HasColumnType("TEXT");

                    b.Property<int?>("RecordingDurationSeconds")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RecordingUrl")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ExternalId")
                        .IsUnique();

                    b.ToTable("Calls");
                });
#pragma warning restore 612, 618
        }
    }
}