using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendorRiskScoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vendor_profile_document_document_id",
                table: "vendor_profile");

            migrationBuilder.DropIndex(
                name: "IX_vendor_profile_document_id",
                table: "vendor_profile");

            migrationBuilder.DropColumn(
                name: "document_id",
                table: "vendor_profile");

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "document",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_document_VendorId",
                table: "document",
                column: "VendorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_document_vendor_profile_VendorId",
                table: "document",
                column: "VendorId",
                principalTable: "vendor_profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_document_vendor_profile_VendorId",
                table: "document");

            migrationBuilder.DropIndex(
                name: "IX_document_VendorId",
                table: "document");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "document");

            migrationBuilder.AddColumn<int>(
                name: "document_id",
                table: "vendor_profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_vendor_profile_document_id",
                table: "vendor_profile",
                column: "document_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_vendor_profile_document_document_id",
                table: "vendor_profile",
                column: "document_id",
                principalTable: "document",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
