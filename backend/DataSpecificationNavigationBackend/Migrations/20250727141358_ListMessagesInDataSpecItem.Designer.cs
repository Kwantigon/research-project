﻿// <auto-generated />
using System;
using DataSpecificationNavigationBackend.ConnectorsLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataSpecificationNavigationBackend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250727141358_ListMessagesInDataSpecItem")]
    partial class ListMessagesInDataSpecItem
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.7")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true);

            modelBuilder.Entity("DataSpecificationItemMessage", b =>
                {
                    b.Property<Guid>("MessagesId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RelatedItemsIri")
                        .HasColumnType("TEXT");

                    b.Property<int>("RelatedItemsDataSpecificationId")
                        .HasColumnType("INTEGER");

                    b.HasKey("MessagesId", "RelatedItemsIri", "RelatedItemsDataSpecificationId");

                    b.HasIndex("RelatedItemsIri", "RelatedItemsDataSpecificationId");

                    b.ToTable("DataSpecificationItemMessage");
                });

            modelBuilder.Entity("DataSpecificationNavigationBackend.Model.Conversation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DataSpecificationId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DataSpecificationId");

                    b.ToTable("Conversations");
                });

            modelBuilder.Entity("DataSpecificationNavigationBackend.Model.DataSpecification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DataspecerPackageUuid")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Owl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DataSpecifications");
                });

            modelBuilder.Entity("DataSpecificationNavigationBackend.Model.DataSpecificationItem", b =>
                {
                    b.Property<string>("Iri")
                        .HasColumnType("TEXT");

                    b.Property<int>("DataSpecificationId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Summary")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Iri", "DataSpecificationId");

                    b.HasIndex("DataSpecificationId");

                    b.ToTable("DataSpecificationItems");
                });

            modelBuilder.Entity("DataSpecificationNavigationBackend.Model.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("ConversationId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("ReplyMessageId")
                        .HasColumnType("TEXT");

                    b.Property<string>("TextValue")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ConversationId");

                    b.HasIndex("ReplyMessageId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("DataSpecificationItemMessage", b =>
                {
                    b.HasOne("DataSpecificationNavigationBackend.Model.Message", null)
                        .WithMany()
                        .HasForeignKey("MessagesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataSpecificationNavigationBackend.Model.DataSpecificationItem", null)
                        .WithMany()
                        .HasForeignKey("RelatedItemsIri", "RelatedItemsDataSpecificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataSpecificationNavigationBackend.Model.Conversation", b =>
                {
                    b.HasOne("DataSpecificationNavigationBackend.Model.DataSpecification", "DataSpecification")
                        .WithMany()
                        .HasForeignKey("DataSpecificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DataSpecification");
                });

            modelBuilder.Entity("DataSpecificationNavigationBackend.Model.DataSpecificationItem", b =>
                {
                    b.HasOne("DataSpecificationNavigationBackend.Model.DataSpecification", "DataSpecification")
                        .WithMany()
                        .HasForeignKey("DataSpecificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DataSpecification");
                });

            modelBuilder.Entity("DataSpecificationNavigationBackend.Model.Message", b =>
                {
                    b.HasOne("DataSpecificationNavigationBackend.Model.Conversation", "Conversation")
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataSpecificationNavigationBackend.Model.Message", "ReplyMessage")
                        .WithMany()
                        .HasForeignKey("ReplyMessageId");

                    b.Navigation("Conversation");

                    b.Navigation("ReplyMessage");
                });

            modelBuilder.Entity("DataSpecificationNavigationBackend.Model.Conversation", b =>
                {
                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
