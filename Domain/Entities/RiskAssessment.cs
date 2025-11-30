using System.ComponentModel.DataAnnotations;

namespace VendorRiskScoreAPI.Domain.Entities
{
    public class RiskAssessment
    {
        [Key]
        public int Id { get; set; }
        public decimal RiskScore { get; set; }
        public string RiskLevel { get; set; }

        //ForeignKey
        public int VendorId { get; set; }
        //NavigationProperty
        public VendorProfile VendorProfile { get; set; }
    }
}
