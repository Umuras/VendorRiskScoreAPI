using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Domain.ValueObjects;
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
        decimal CalculateFinalScore(int financialHealth, int slaUpTime, int majorIncidents,
           List<VendorSecurityCert> vendorSecurityCerts, Domain.Entities.Document document);
        string CalculateRiskLevel(decimal riskScore);
        Task<RiskAssessmentResponseDto> CreateRiskAssessmentResult(VendorProfile vendorProfile);
        Task<RiskScores> CalculateVendorProfileRiskScores(string vendorProfileName, int financialHealth, int slaUpTime, int majorIncidents,
           List<VendorSecurityCert> vendorSecurityCerts, Domain.Entities.Document document);
    }
}
