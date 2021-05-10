﻿// <auto-generated />
using System;
using CivicCommunicator.DataAccess.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CivicCommunicator.Migrations
{
    [DbContext(typeof(CivicBotDbContext))]
    [Migration("20200221193020_Add table for Bot Replies")]
    partial class AddtableforBotReplies
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CivicCommunicator.DataAccess.DataModel.Models.BotReply", b =>
                {
                    b.Property<int>("BotReplyId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Text");

                    b.Property<int>("ToId");

                    b.HasKey("BotReplyId");

                    b.HasIndex("ToId");

                    b.ToTable("BotReplies");
                });

            modelBuilder.Entity("CivicCommunicator.DataAccess.DataModel.Models.ConversationRequest", b =>
                {
                    b.Property<int>("ConversationRequestId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AgentId");

                    b.Property<DateTime>("CreationDate");

                    b.Property<int>("RequesterId");

                    b.Property<string>("State")
                        .IsRequired();

                    b.HasKey("ConversationRequestId");

                    b.HasIndex("AgentId");

                    b.HasIndex("RequesterId");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("CivicCommunicator.DataAccess.DataModel.Models.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate");

                    b.Property<int>("FromId");

                    b.Property<string>("Text");

                    b.HasKey("MessageId");

                    b.HasIndex("FromId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("CivicCommunicator.DataAccess.DataModel.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BotChannelId");

                    b.Property<string>("ChannelId");

                    b.Property<string>("ChatId");

                    b.Property<string>("ConversationId");

                    b.Property<string>("HandlingDomain");

                    b.Property<bool>("IsAgent");

                    b.Property<bool>("IsOnline");

                    b.Property<string>("Name");

                    b.Property<string>("ServiceUrl");

                    b.Property<string>("SiteDomain");

                    b.HasKey("UserId");

                    b.HasIndex("ChatId")
                        .IsUnique()
                        .HasFilter("[ChatId] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CivicCommunicator.DataAccess.DataModel.Models.BotReply", b =>
                {
                    b.HasOne("CivicCommunicator.DataAccess.DataModel.Models.User", "To")
                        .WithMany("BotReplies")
                        .HasForeignKey("ToId");
                });

            modelBuilder.Entity("CivicCommunicator.DataAccess.DataModel.Models.ConversationRequest", b =>
                {
                    b.HasOne("CivicCommunicator.DataAccess.DataModel.Models.User", "Agent")
                        .WithMany("Handled")
                        .HasForeignKey("AgentId");

                    b.HasOne("CivicCommunicator.DataAccess.DataModel.Models.User", "Requester")
                        .WithMany("Requested")
                        .HasForeignKey("RequesterId");
                });

            modelBuilder.Entity("CivicCommunicator.DataAccess.DataModel.Models.Message", b =>
                {
                    b.HasOne("CivicCommunicator.DataAccess.DataModel.Models.User", "From")
                        .WithMany("Messages")
                        .HasForeignKey("FromId");
                });
#pragma warning restore 612, 618
        }
    }
}
