using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Services
{
    public interface IRiskAssessmentService
    {
        Task<List<RiskAssessment>> GetAllRiskAssessmentsAsync();
        Task<RiskAssessment> GetRiskAssessmentByIdAsync(int id);
        Task AddRiskAssessmentAsync(RiskAssessment riskAssessment);
        Task UpdateRiskAssessmentAsync(int id, RiskAssessment riskAssessment);
        Task DeleteRiskAssessmentAsync(int id);
    }
}
