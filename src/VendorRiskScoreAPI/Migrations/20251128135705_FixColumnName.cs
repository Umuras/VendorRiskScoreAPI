using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendorRiskScoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_document_vendor_profile_VendorId",
                table: "document");

            migrationBuilder.RenameColumn(
                name: "VendorId",
                table: "document",
                newName: "vendor_id");

            migrationBuilder.RenameIndex(
                name: "IX_document_VendorId",
                table: "document",
                newName: "IX_document_vendor_id");

            migrationBuilder.AddForeignKey(
                name: "FK_document_vendor_profile_vendor_id",
                table: "document",
                column: "vendor_id",
                principalTable: "vendor_profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_document_vendor_profile_vendor_id",
                table: "document");

            migrationBuilder.RenameColumn(
                name: "vendor_id",
                table: "document",
                newName: "VendorId");

            migrationBuilder.RenameIndex(
                name: "IX_document_vendor_id",
                table: "document",
                newName: "IX_document_VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_document_vendor_profile_VendorId",
                table: "document",
                column: "VendorId",
                principalTable: "vendor_profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
