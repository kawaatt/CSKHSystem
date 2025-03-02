﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TELEBOT_CSKH.Data;

#nullable disable

namespace TELEBOT_CSKH.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250301121011_updatedb6")]
    partial class updatedb6
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TELEBOT_CSKH.Models.TELEGRAM_BOT.TelegramAccount", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("BotType")
                        .HasColumnType("int");

                    b.Property<string>("ChatID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsIndividualWorking")
                        .HasColumnType("bit");

                    b.Property<string>("KeyboardData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<string>("System")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("URLHooking")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("fRunCampaign")
                        .HasColumnType("bit");

                    b.HasKey("ID");

                    b.ToTable("TelegramAccount");
                });

            modelBuilder.Entity("TELEBOT_CSKH.Models.TELEGRAM_BOT.TelegramCampaign", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("IDBot")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImageURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InlineKeyboard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("iRun")
                        .HasColumnType("bit");

                    b.HasKey("ID");

                    b.ToTable("TelegramCampaign");
                });

            modelBuilder.Entity("TELEBOT_CSKH.Models.TELEGRAM_BOT.TelegramCustomer", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("BotAffiliateID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BotID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ShareCount")
                        .HasColumnType("int");

                    b.Property<string>("System")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TelegramID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("iPremium")
                        .HasColumnType("bit");

                    b.HasKey("ID");

                    b.ToTable("TelegramCustomers");
                });

            modelBuilder.Entity("TELEBOT_CSKH.Models.TELEGRAM_BOT.TelegramResponse", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BotID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("InlineKeyboard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RequestCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("URLImage")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("TelegramResponse");
                });
#pragma warning restore 612, 618
        }
    }
}
