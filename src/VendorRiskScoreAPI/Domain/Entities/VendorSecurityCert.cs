using System.ComponentModel.DataAnnotations;

namespace VendorRiskScoreAPI.Domain.Entities
{
    public class VendorSecurityCert
    {
        [Key]
        public int Id { get; set; }
        public string CertName { get; set; }

        //Navigation Property
        public VendorProfile VendorProfile { get; set; }
        //Foreign Key
        public int VendorId { get; set; }
    }
}
