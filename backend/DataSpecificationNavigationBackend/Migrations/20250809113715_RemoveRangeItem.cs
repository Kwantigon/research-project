using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataSpecificationNavigationBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRangeItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataSpecificationItemSuggestions_DataSpecificationItems_RangeItemIri_ItemDataSpecificationId",
                table: "DataSpecificationItemSuggestions");

            migrationBuilder.DropIndex(
                name: "IX_DataSpecificationItemSuggestions_RangeItemIri_ItemDataSpecificationId",
                table: "DataSpecificationItemSuggestions");

            migrationBuilder.AlterColumn<string>(
                name: "RangeItemIri",
                table: "DataSpecificationItemSuggestions",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RangeItemIri",
                table: "DataSpecificationItemSuggestions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_DataSpecificationItemSuggestions_RangeItemIri_ItemDataSpecificationId",
                table: "DataSpecificationItemSuggestions",
                columns: new[] { "RangeItemIri", "ItemDataSpecificationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DataSpecificationItemSuggestions_DataSpecificationItems_RangeItemIri_ItemDataSpecificationId",
                table: "DataSpecificationItemSuggestions",
                columns: new[] { "RangeItemIri", "ItemDataSpecificationId" },
                principalTable: "DataSpecificationItems",
                principalColumns: new[] { "Iri", "DataSpecificationId" },
                onDelete: ReferentialAction.SetNull);
        }
    }
}
