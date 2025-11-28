using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Dtos
{
    public class VendorProfileRequestDto
    {
        public string Name { get; set; }
        public int FinancialHealth { get; set; }
        public int SlaUpTime { get; set; }
        public int MajorIncidents { get; set; }

        public ICollection<string> SecurityCerts { get; set; } = new List<string>();
        public DocumentRequestDto Documents { get; set; }
    }
}
