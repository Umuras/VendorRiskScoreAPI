using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Dtos
{
    public class VendorProfileRequestDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(maximumLength: 25, ErrorMessage = "You can write name only 3 to 25 characters", MinimumLength = 3)]
        [DefaultValue("")]
        public string Name { get; set; }

        [Required(ErrorMessage = "FinancialHealth is required")]
        [Range(0, 100, ErrorMessage = "FinancialHealth must be between 0 and 100")]
        [DefaultValue(0)]
        public int FinancialHealth { get; set; }

        [Required(ErrorMessage = "SlaUptime is required")]
        [Range(0, 100, ErrorMessage = "SlaUptime must be between 0 and 100")]
        [DefaultValue(0)]
        public int SlaUptime { get; set; }
        [Required(ErrorMessage = "MajorIncidents is required")]
        [Range(0, 12, ErrorMessage = "MajorIncidents must be between 0 and 12")]
        [DefaultValue(0)]
        public int MajorIncidents { get; set; }

        public ICollection<string> SecurityCerts { get; set; } = new List<string>();
        public DocumentDto Documents { get; set; }
    }
}
