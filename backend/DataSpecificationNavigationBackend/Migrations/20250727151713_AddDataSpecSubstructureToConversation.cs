using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataSpecificationNavigationBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddDataSpecSubstructureToConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConversationId",
                table: "DataSpecificationItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserSelectedItems",
                table: "Conversations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataSpecificationItems_ConversationId",
                table: "DataSpecificationItems",
                column: "ConversationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataSpecificationItems_Conversations_ConversationId",
                table: "DataSpecificationItems",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataSpecificationItems_Conversations_ConversationId",
                table: "DataSpecificationItems");

            migrationBuilder.DropIndex(
                name: "IX_DataSpecificationItems_ConversationId",
                table: "DataSpecificationItems");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "DataSpecificationItems");

            migrationBuilder.DropColumn(
                name: "UserSelectedItems",
                table: "Conversations");
        }
    }
}
