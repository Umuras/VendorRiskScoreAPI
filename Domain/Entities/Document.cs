using System.ComponentModel.DataAnnotations;

namespace VendorRiskScoreAPI.Domain.Entities
{
    public class Document
    {
        [Key]
        public int Id { get; set; }
        public bool ContractValid { get; set; }
        public bool PrivacyPolicyValid { get; set; }
        public bool PentestReportValid { get; set; }

        //Foreign Key
        public int VendorId { get; set; }
        //Navigation Property
        public VendorProfile VendorProfile { get; set; }
    }
}
