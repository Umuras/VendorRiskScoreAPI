using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VendorRiskScoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorProfileRiskScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "risk_score",
                table: "risk_assessment",
                type: "numeric(5,4)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.CreateTable(
                name: "vendor_profile_risk_score",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VendorName = table.Column<string>(type: "text", nullable: false),
                    financial = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    operational = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    security = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    final_score = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    calculated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    vendor_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vendor_profile_risk_score", x => x.id);
                    table.ForeignKey(
                        name: "FK_vendor_profile_risk_score_vendor_profile_vendor_id",
                        column: x => x.vendor_id,
                        principalTable: "vendor_profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_vendor_profile_risk_score_vendor_id",
                table: "vendor_profile_risk_score",
                column: "vendor_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vendor_profile_risk_score");

            migrationBuilder.AlterColumn<float>(
                name: "risk_score",
                table: "risk_assessment",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,4)");
        }
    }
}
