using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataSpecificationNavigationBackend.Migrations
{
    /// <inheritdoc />
    public partial class ListMessagesInDataSpecItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataSpecificationItems_Messages_MessageId",
                table: "DataSpecificationItems");

            migrationBuilder.DropIndex(
                name: "IX_DataSpecificationItems_MessageId",
                table: "DataSpecificationItems");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "DataSpecificationItems");

            migrationBuilder.CreateTable(
                name: "DataSpecificationItemMessage",
                columns: table => new
                {
                    MessagesId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RelatedItemsIri = table.Column<string>(type: "TEXT", nullable: false),
                    RelatedItemsDataSpecificationId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSpecificationItemMessage", x => new { x.MessagesId, x.RelatedItemsIri, x.RelatedItemsDataSpecificationId });
                    table.ForeignKey(
                        name: "FK_DataSpecificationItemMessage_DataSpecificationItems_RelatedItemsIri_RelatedItemsDataSpecificationId",
                        columns: x => new { x.RelatedItemsIri, x.RelatedItemsDataSpecificationId },
                        principalTable: "DataSpecificationItems",
                        principalColumns: new[] { "Iri", "DataSpecificationId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataSpecificationItemMessage_Messages_MessagesId",
                        column: x => x.MessagesId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataSpecificationItemMessage_RelatedItemsIri_RelatedItemsDataSpecificationId",
                table: "DataSpecificationItemMessage",
                columns: new[] { "RelatedItemsIri", "RelatedItemsDataSpecificationId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataSpecificationItemMessage");

            migrationBuilder.AddColumn<Guid>(
                name: "MessageId",
                table: "DataSpecificationItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataSpecificationItems_MessageId",
                table: "DataSpecificationItems",
                column: "MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataSpecificationItems_Messages_MessageId",
                table: "DataSpecificationItems",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id");
        }
    }
}
