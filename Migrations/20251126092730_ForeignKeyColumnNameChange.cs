using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendorRiskScoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKeyColumnNameChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_risk_assessment_vendor_profile_VendorId",
                table: "risk_assessment");

            migrationBuilder.DropForeignKey(
                name: "FK_vendor_profile_document_DocumentId",
                table: "vendor_profile");

            migrationBuilder.DropForeignKey(
                name: "FK_vendor_security_cert_vendor_profile_VendorId",
                table: "vendor_security_cert");

            migrationBuilder.RenameColumn(
                name: "VendorId",
                table: "vendor_security_cert",
                newName: "vendor_id");

            migrationBuilder.RenameIndex(
                name: "IX_vendor_security_cert_VendorId",
                table: "vendor_security_cert",
                newName: "IX_vendor_security_cert_vendor_id");

            migrationBuilder.RenameColumn(
                name: "DocumentId",
                table: "vendor_profile",
                newName: "document_id");

            migrationBuilder.RenameIndex(
                name: "IX_vendor_profile_DocumentId",
                table: "vendor_profile",
                newName: "IX_vendor_profile_document_id");

            migrationBuilder.RenameColumn(
                name: "VendorId",
                table: "risk_assessment",
                newName: "vendor_id");

            migrationBuilder.RenameIndex(
                name: "IX_risk_assessment_VendorId",
                table: "risk_assessment",
                newName: "IX_risk_assessment_vendor_id");

            migrationBuilder.AddForeignKey(
                name: "FK_risk_assessment_vendor_profile_vendor_id",
                table: "risk_assessment",
                column: "vendor_id",
                principalTable: "vendor_profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_vendor_profile_document_document_id",
                table: "vendor_profile",
                column: "document_id",
                principalTable: "document",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_vendor_security_cert_vendor_profile_vendor_id",
                table: "vendor_security_cert",
                column: "vendor_id",
                principalTable: "vendor_profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_risk_assessment_vendor_profile_vendor_id",
                table: "risk_assessment");

            migrationBuilder.DropForeignKey(
                name: "FK_vendor_profile_document_document_id",
                table: "vendor_profile");

            migrationBuilder.DropForeignKey(
                name: "FK_vendor_security_cert_vendor_profile_vendor_id",
                table: "vendor_security_cert");

            migrationBuilder.RenameColumn(
                name: "vendor_id",
                table: "vendor_security_cert",
                newName: "VendorId");

            migrationBuilder.RenameIndex(
                name: "IX_vendor_security_cert_vendor_id",
                table: "vendor_security_cert",
                newName: "IX_vendor_security_cert_VendorId");

            migrationBuilder.RenameColumn(
                name: "document_id",
                table: "vendor_profile",
                newName: "DocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_vendor_profile_document_id",
                table: "vendor_profile",
                newName: "IX_vendor_profile_DocumentId");

            migrationBuilder.RenameColumn(
                name: "vendor_id",
                table: "risk_assessment",
                newName: "VendorId");

            migrationBuilder.RenameIndex(
                name: "IX_risk_assessment_vendor_id",
                table: "risk_assessment",
                newName: "IX_risk_assessment_VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_risk_assessment_vendor_profile_VendorId",
                table: "risk_assessment",
                column: "VendorId",
                principalTable: "vendor_profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_vendor_profile_document_DocumentId",
                table: "vendor_profile",
                column: "DocumentId",
                principalTable: "document",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_vendor_security_cert_vendor_profile_VendorId",
                table: "vendor_security_cert",
                column: "VendorId",
                principalTable: "vendor_profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
