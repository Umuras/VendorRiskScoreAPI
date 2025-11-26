using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendorRiskScoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnsOrFixFluentAPI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RiskAssessment_VendorProfile_VendorId",
                table: "RiskAssessment");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorProfile_Document_DocumentId",
                table: "VendorProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorSecurityCert_VendorProfile_VendorId",
                table: "VendorSecurityCert");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Document",
                table: "Document");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VendorSecurityCert",
                table: "VendorSecurityCert");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VendorProfile",
                table: "VendorProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RiskAssessment",
                table: "RiskAssessment");

            migrationBuilder.RenameTable(
                name: "Document",
                newName: "document");

            migrationBuilder.RenameTable(
                name: "VendorSecurityCert",
                newName: "vendor_security_cert");

            migrationBuilder.RenameTable(
                name: "VendorProfile",
                newName: "vendor_profile");

            migrationBuilder.RenameTable(
                name: "RiskAssessment",
                newName: "risk_assessment");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "document",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PrivacyPolicyValid",
                table: "document",
                newName: "privacy_policy_valid");

            migrationBuilder.RenameColumn(
                name: "PentestReportValid",
                table: "document",
                newName: "pentest_report_valid");

            migrationBuilder.RenameColumn(
                name: "ContractValid",
                table: "document",
                newName: "contract_valid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "vendor_security_cert",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CertName",
                table: "vendor_security_cert",
                newName: "cert_name");

            migrationBuilder.RenameIndex(
                name: "IX_VendorSecurityCert_VendorId",
                table: "vendor_security_cert",
                newName: "IX_vendor_security_cert_VendorId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "vendor_profile",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "vendor_profile",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SlaUpTime",
                table: "vendor_profile",
                newName: "sla_up_time");

            migrationBuilder.RenameColumn(
                name: "MajorIncidents",
                table: "vendor_profile",
                newName: "major_incidents");

            migrationBuilder.RenameColumn(
                name: "FinancialHealth",
                table: "vendor_profile",
                newName: "financial_health");

            migrationBuilder.RenameIndex(
                name: "IX_VendorProfile_DocumentId",
                table: "vendor_profile",
                newName: "IX_vendor_profile_DocumentId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "risk_assessment",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RiskScore",
                table: "risk_assessment",
                newName: "risk_score");

            migrationBuilder.RenameColumn(
                name: "RiskLevel",
                table: "risk_assessment",
                newName: "risk_level");

            migrationBuilder.RenameIndex(
                name: "IX_RiskAssessment_VendorId",
                table: "risk_assessment",
                newName: "IX_risk_assessment_VendorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_document",
                table: "document",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_vendor_security_cert",
                table: "vendor_security_cert",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_vendor_profile",
                table: "vendor_profile",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_risk_assessment",
                table: "risk_assessment",
                column: "id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_document",
                table: "document");

            migrationBuilder.DropPrimaryKey(
                name: "PK_vendor_security_cert",
                table: "vendor_security_cert");

            migrationBuilder.DropPrimaryKey(
                name: "PK_vendor_profile",
                table: "vendor_profile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_risk_assessment",
                table: "risk_assessment");

            migrationBuilder.RenameTable(
                name: "document",
                newName: "Document");

            migrationBuilder.RenameTable(
                name: "vendor_security_cert",
                newName: "VendorSecurityCert");

            migrationBuilder.RenameTable(
                name: "vendor_profile",
                newName: "VendorProfile");

            migrationBuilder.RenameTable(
                name: "risk_assessment",
                newName: "RiskAssessment");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Document",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "privacy_policy_valid",
                table: "Document",
                newName: "PrivacyPolicyValid");

            migrationBuilder.RenameColumn(
                name: "pentest_report_valid",
                table: "Document",
                newName: "PentestReportValid");

            migrationBuilder.RenameColumn(
                name: "contract_valid",
                table: "Document",
                newName: "ContractValid");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "VendorSecurityCert",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "cert_name",
                table: "VendorSecurityCert",
                newName: "CertName");

            migrationBuilder.RenameIndex(
                name: "IX_vendor_security_cert_VendorId",
                table: "VendorSecurityCert",
                newName: "IX_VendorSecurityCert_VendorId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "VendorProfile",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "VendorProfile",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "sla_up_time",
                table: "VendorProfile",
                newName: "SlaUpTime");

            migrationBuilder.RenameColumn(
                name: "major_incidents",
                table: "VendorProfile",
                newName: "MajorIncidents");

            migrationBuilder.RenameColumn(
                name: "financial_health",
                table: "VendorProfile",
                newName: "FinancialHealth");

            migrationBuilder.RenameIndex(
                name: "IX_vendor_profile_DocumentId",
                table: "VendorProfile",
                newName: "IX_VendorProfile_DocumentId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RiskAssessment",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "risk_score",
                table: "RiskAssessment",
                newName: "RiskScore");

            migrationBuilder.RenameColumn(
                name: "risk_level",
                table: "RiskAssessment",
                newName: "RiskLevel");

            migrationBuilder.RenameIndex(
                name: "IX_risk_assessment_VendorId",
                table: "RiskAssessment",
                newName: "IX_RiskAssessment_VendorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Document",
                table: "Document",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VendorSecurityCert",
                table: "VendorSecurityCert",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VendorProfile",
                table: "VendorProfile",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RiskAssessment",
                table: "RiskAssessment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RiskAssessment_VendorProfile_VendorId",
                table: "RiskAssessment",
                column: "VendorId",
                principalTable: "VendorProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorProfile_Document_DocumentId",
                table: "VendorProfile",
                column: "DocumentId",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorSecurityCert_VendorProfile_VendorId",
                table: "VendorSecurityCert",
                column: "VendorId",
                principalTable: "VendorProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
