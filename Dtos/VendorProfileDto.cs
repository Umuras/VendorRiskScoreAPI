using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Dtos
{
    public class VendorProfileDto
    {
        public string Name { get; set; }
        public int FinancialHealth { get; set; }
        public int SlaUptime { get; set; }
        public int MajorIncidents { get; set; }

        public ICollection<string> SecurityCerts { get; set; } = new List<string>();
        public DocumentDto Documents { get; set; }
    }
}
