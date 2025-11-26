using Microsoft.EntityFrameworkCore;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Repositories
{
    public class RiskAssessmentRepository : IRiskAssessmentRepository
    {
        private readonly VendorRiskScoreDbContext _context;

        public RiskAssessmentRepository(VendorRiskScoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<RiskAssessment>> GetAllRiskAssessmentsAsync()
        {
            return await _context.RiskAssessments.ToListAsync();
        }

        public async Task<RiskAssessment> GetRiskAssessmentByIdAsync(int id)
        {
            RiskAssessment? riskAssessment = await _context.RiskAssessments.FirstOrDefaultAsync(r => r.Id == id);
            return riskAssessment;
        }

        public async Task AddRiskAssessmentAsync(RiskAssessment riskAssessment)
        {
            await _context.RiskAssessments.AddAsync(riskAssessment);
        }

        public Task UpdateRiskAssessmentAsync(RiskAssessment riskAssessment)
        {
            _context.RiskAssessments.Update(riskAssessment);
            return Task.CompletedTask;
        }

        public Task DeleteRiskAssessmentAsync(RiskAssessment riskAssessment)
        {
            _context.RiskAssessments.Remove(riskAssessment);
            return Task.CompletedTask;
        }
    }
}
