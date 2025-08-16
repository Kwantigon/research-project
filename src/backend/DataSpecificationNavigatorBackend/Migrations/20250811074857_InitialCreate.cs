using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataSpecificationNavigatorBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataSpecifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataspecerPackageUuid = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Owl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataSpecificationId = table.Column<int>(type: "INTEGER", nullable: false),
                    SubstructureJsonString = table.Column<string>(type: "TEXT", nullable: false),
                    UserSelectedItems = table.Column<string>(type: "TEXT", nullable: false),
                    SuggestedMessage = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversations_DataSpecifications_DataSpecificationId",
                        column: x => x.DataSpecificationId,
                        principalTable: "DataSpecifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataSpecificationItems",
                columns: table => new
                {
                    Iri = table.Column<string>(type: "TEXT", nullable: false),
                    DataSpecificationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Label = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: true),
                    DomainItemIri = table.Column<string>(type: "TEXT", nullable: true),
                    RangeItemIri = table.Column<string>(type: "TEXT", nullable: true),
                    RangeItemDataSpecificationId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSpecificationItems", x => new { x.Iri, x.DataSpecificationId });
                    table.ForeignKey(
                        name: "FK_DataSpecificationItems_DataSpecificationItems_DomainItemIri_DataSpecificationId",
                        columns: x => new { x.DomainItemIri, x.DataSpecificationId },
                        principalTable: "DataSpecificationItems",
                        principalColumns: new[] { "Iri", "DataSpecificationId" },
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DataSpecificationItems_DataSpecificationItems_RangeItemIri_RangeItemDataSpecificationId",
                        columns: x => new { x.RangeItemIri, x.RangeItemDataSpecificationId },
                        principalTable: "DataSpecificationItems",
                        principalColumns: new[] { "Iri", "DataSpecificationId" });
                    table.ForeignKey(
                        name: "FK_DataSpecificationItems_DataSpecifications_DataSpecificationId",
                        column: x => x.DataSpecificationId,
                        principalTable: "DataSpecifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Sender = table.Column<int>(type: "INTEGER", nullable: false),
                    TextContent = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConversationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    IsGenerated = table.Column<bool>(type: "INTEGER", nullable: true),
                    MappingText = table.Column<string>(type: "TEXT", nullable: true),
                    SparqlText = table.Column<string>(type: "TEXT", nullable: true),
                    SparqlQuery = table.Column<string>(type: "TEXT", nullable: true),
                    SuggestItemsText = table.Column<string>(type: "TEXT", nullable: true),
                    ReplyMessageId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Messages_ReplyMessageId",
                        column: x => x.ReplyMessageId,
                        principalTable: "Messages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DataSpecificationItemMappings",
                columns: table => new
                {
                    ItemIri = table.Column<string>(type: "TEXT", nullable: false),
                    ItemDataSpecificationId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserMessageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MappedWords = table.Column<string>(type: "TEXT", nullable: false),
                    IsSelectTarget = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSpecificationItemMappings", x => new { x.ItemDataSpecificationId, x.ItemIri, x.UserMessageId });
                    table.ForeignKey(
                        name: "FK_DataSpecificationItemMappings_DataSpecificationItems_ItemIri_ItemDataSpecificationId",
                        columns: x => new { x.ItemIri, x.ItemDataSpecificationId },
                        principalTable: "DataSpecificationItems",
                        principalColumns: new[] { "Iri", "DataSpecificationId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataSpecificationItemMappings_Messages_UserMessageId",
                        column: x => x.UserMessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataSpecificationItemSuggestions",
                columns: table => new
                {
                    ItemDataSpecificationId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemIri = table.Column<string>(type: "TEXT", nullable: false),
                    ReplyMessageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReasonForSuggestion = table.Column<string>(type: "TEXT", nullable: false),
                    DomainItemIri = table.Column<string>(type: "TEXT", nullable: true),
                    RangeItemIri = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSpecificationItemSuggestions", x => new { x.ItemDataSpecificationId, x.ItemIri, x.ReplyMessageId });
                    table.ForeignKey(
                        name: "FK_DataSpecificationItemSuggestions_DataSpecificationItems_DomainItemIri_ItemDataSpecificationId",
                        columns: x => new { x.DomainItemIri, x.ItemDataSpecificationId },
                        principalTable: "DataSpecificationItems",
                        principalColumns: new[] { "Iri", "DataSpecificationId" },
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DataSpecificationItemSuggestions_DataSpecificationItems_ItemIri_ItemDataSpecificationId",
                        columns: x => new { x.ItemIri, x.ItemDataSpecificationId },
                        principalTable: "DataSpecificationItems",
                        principalColumns: new[] { "Iri", "DataSpecificationId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataSpecificationItemSuggestions_Messages_ReplyMessageId",
                        column: x => x.ReplyMessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_DataSpecificationId",
                table: "Conversations",
                column: "DataSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSpecificationItemMappings_ItemIri_ItemDataSpecificationId",
                table: "DataSpecificationItemMappings",
                columns: new[] { "ItemIri", "ItemDataSpecificationId" });

            migrationBuilder.CreateIndex(
                name: "IX_DataSpecificationItemMappings_UserMessageId",
                table: "DataSpecificationItemMappings",
                column: "UserMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSpecificationItems_DataSpecificationId",
                table: "DataSpecificationItems",
                column: "DataSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSpecificationItems_DomainItemIri_DataSpecificationId",
                table: "DataSpecificationItems",
                columns: new[] { "DomainItemIri", "DataSpecificationId" });

            migrationBuilder.CreateIndex(
                name: "IX_DataSpecificationItems_RangeItemIri_RangeItemDataSpecificationId",
                table: "DataSpecificationItems",
                columns: new[] { "RangeItemIri", "RangeItemDataSpecificationId" });

            migrationBuilder.CreateIndex(
                name: "IX_DataSpecificationItemSuggestions_DomainItemIri_ItemDataSpecificationId",
                table: "DataSpecificationItemSuggestions",
                columns: new[] { "DomainItemIri", "ItemDataSpecificationId" });

            migrationBuilder.CreateIndex(
                name: "IX_DataSpecificationItemSuggestions_ItemIri_ItemDataSpecificationId",
                table: "DataSpecificationItemSuggestions",
                columns: new[] { "ItemIri", "ItemDataSpecificationId" });

            migrationBuilder.CreateIndex(
                name: "IX_DataSpecificationItemSuggestions_ReplyMessageId",
                table: "DataSpecificationItemSuggestions",
                column: "ReplyMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReplyMessageId",
                table: "Messages",
                column: "ReplyMessageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataSpecificationItemMappings");

            migrationBuilder.DropTable(
                name: "DataSpecificationItemSuggestions");

            migrationBuilder.DropTable(
                name: "DataSpecificationItems");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "DataSpecifications");
        }
    }
}
