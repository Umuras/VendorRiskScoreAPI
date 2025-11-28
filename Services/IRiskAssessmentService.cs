using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Dtos;

namespace VendorRiskScoreAPI.Services
{
    public interface IRiskAssessmentService
    {
        Task<List<RiskAssessment>> GetAllRiskAssessmentsAsync();
        Task<RiskAssessment> GetRiskAssessmentByIdAsync(int id);
        Task AddRiskAssessmentAsync(RiskAssessment riskAssessment);
        Task UpdateRiskAssessmentAsync(int id, RiskAssessment riskAssessment);
        Task DeleteRiskAssessmentAsync(int id);
        double CalculateFinalScore(int financialHealth, int slaUpTime, int majorIncidents,
           List<VendorSecurityCert> vendorSecurityCerts, Domain.Entities.Document document);
        string CalculateRiskLevel(double riskScore);
        Task<RiskAssessmentResponseDto> CreateRiskAssessmentResult(VendorProfile vendorProfile);
    }
}
