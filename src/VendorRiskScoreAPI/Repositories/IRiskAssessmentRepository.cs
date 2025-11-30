using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Repositories
{
    public interface IRiskAssessmentRepository
    {
        Task<List<RiskAssessment>> GetAllRiskAssessmentsAsync();
        Task<RiskAssessment> GetRiskAssessmentByIdAsync(int id);
        Task AddRiskAssessmentAsync(RiskAssessment riskAssessment);
        Task UpdateRiskAssessmentAsync(RiskAssessment riskAssessment);
        Task DeleteRiskAssessmentAsync(RiskAssessment riskAssessment);
    }
}
