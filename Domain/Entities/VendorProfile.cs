using System.ComponentModel.DataAnnotations;

namespace VendorRiskScoreAPI.Domain.Entities
{
    public class VendorProfile
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int FinancialHealth { get; set; }
        public int SlaUpTime { get; set; }
        public int MajorIncidents { get; set; }
        public ICollection<VendorSecurityCert> SecurityCerts { get; set; } = new List<VendorSecurityCert>();
        
        //Navigation Property
        public Document Document { get; set; }
        //Navigation Property
        public RiskAssessment RiskAssessment { get; set; }
    }
}
