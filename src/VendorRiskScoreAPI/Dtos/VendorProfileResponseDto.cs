using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VendorRiskScoreAPI.Dtos
{
    public record VendorProfileResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int FinancialHealth { get; set; }

        public int SlaUptime { get; set; }

        public int MajorIncidents { get; set; }

        public ICollection<string> SecurityCerts { get; set; } = new List<string>();
        public DocumentDto Documents { get; set; }
    }
}
