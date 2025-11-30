using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendorRiskScoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class DeleteCalculateAtVendorProfileRiskScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "calculated_at",
                table: "vendor_profile_risk_score");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "calculated_at",
                table: "vendor_profile_risk_score",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");
        }
    }
}
