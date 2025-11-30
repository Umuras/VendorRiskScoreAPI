using System.ComponentModel.DataAnnotations;

namespace VendorRiskScoreAPI.Domain.Entities
{
    public class VendorProfileRiskScore
    {
        [Key]
        public int Id { get; set; }
        public string VendorName { get; set; }
        public decimal Financial { get; set; }
        public decimal Operational { get; set; }
        public decimal Security { get; set; }
        public decimal FinalScore { get; set; }

        //Navigation Property
        public VendorProfile VendorProfile { get; set; }
        //Foreign Key
        public int VendorId { get; set; }
    }
}
