using Microsoft.EntityFrameworkCore;
using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Data
{
    public class VendorRiskScoreDbContext : DbContext
    {
        public VendorRiskScoreDbContext(DbContextOptions<VendorRiskScoreDbContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<VendorProfile> VendorProfiles { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<VendorSecurityCert> VendorSecurityCerts { get; set; }
        public DbSet<RiskAssessment> RiskAssessments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureVendorProfile(modelBuilder);
            ConfigureVendorSecurityCert(modelBuilder);
            ConfigureDocument(modelBuilder);
            ConfigureRiskAssessment(modelBuilder);
        }

        private void ConfigureVendorProfile(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VendorProfile>(entity =>
            {
                entity.ToTable("vendor_profile")
                      .HasKey(v => v.Id);

                entity.Property(v => v.Id)
                      .IsRequired()
                      .HasColumnName("id");

                entity.Property(v => v.Name)
                      .IsRequired()
                      .HasColumnName("name")
                      .HasMaxLength(200);

                entity.Property(v => v.FinancialHealth)
                      .HasColumnName("financial_health")
                      .IsRequired();

                entity.Property(v => v.SlaUpTime)
                      .HasColumnName("sla_up_time")
                      .IsRequired();

                entity.Property(v => v.MajorIncidents)
                      .HasColumnName("major_incidents")
                      .IsRequired();

                entity.Property(v => v.DocumentId)
                      .HasColumnName("document_id")
                      .IsRequired();

                entity.HasMany(v => v.SecurityCerts)
                      .WithOne(s => s.VendorProfile)
                      .HasForeignKey(s => s.VendorId);

                entity.HasOne(v => v.Document)
                      .WithOne()
                      .HasForeignKey<VendorProfile>(v => v.DocumentId);
            });
        }

        private void ConfigureVendorSecurityCert(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VendorSecurityCert>(entity =>
            {
                entity.ToTable("vendor_security_cert")
                      .HasKey(vs => vs.Id);

                entity.Property(vs => vs.Id)
                      .IsRequired()
                      .HasColumnName("id");

                entity.Property(vs => vs.CertName)
                      .HasColumnName("cert_name")
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(vs => vs.VendorId)
                      .IsRequired()
                      .HasColumnName("vendor_id");

                entity.HasOne(vs => vs.VendorProfile)
                      .WithMany(v => v.SecurityCerts)
                      .HasForeignKey(vs => vs.VendorId);
            });
        }

        private void ConfigureDocument(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("document")
                      .HasKey(d => d.Id);

                entity.Property(d => d.Id)
                      .IsRequired()
                      .HasColumnName("id");

                entity.Property(d => d.ContractValid)
                      .HasColumnName("contract_valid")
                      .IsRequired();

                entity.Property(d => d.PrivacyPolicyValid)
                      .HasColumnName("privacy_policy_valid")
                      .IsRequired();

                entity.Property(d => d.PentestReportValid)
                      .HasColumnName("pentest_report_valid")
                      .IsRequired();
            });
        }

        private void ConfigureRiskAssessment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RiskAssessment>(entity =>
            {
                entity.ToTable("risk_assessment")
                      .HasKey(r => r.Id);

                entity.Property(r => r.Id)
                      .IsRequired()
                      .HasColumnName("id");

                entity.Property(r => r.RiskScore)
                      .HasColumnName("risk_score")
                      .IsRequired();

                entity.Property(r => r.RiskLevel)
                      .HasColumnName("risk_level")
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(r => r.VendorId)
                     .IsRequired()
                     .HasColumnName("vendor_id");

                entity.HasOne(r => r.VendorProfile)
                      .WithOne(v => v.RiskAssessment)
                      .HasForeignKey<RiskAssessment>(r => r.VendorId);
            });
        }
    }
}
