using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Dtos
{
    public class VendorProfileRequestDto
    {
        public string Name { get; set; }
        public int FinancialHealth { get; set; }
        public int SlaUptime { get; set; }
        public int MajorIncidents { get; set; }

        public ICollection<VendorSecurityCertRequestDto> SecurityCerts { get; set; } = new List<VendorSecurityCertRequestDto>();
        public DocumentRequestDto Documents { get; set; }
    }
}
