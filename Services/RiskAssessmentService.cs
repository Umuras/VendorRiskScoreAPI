using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection.Metadata;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Repositories;

namespace VendorRiskScoreAPI.Services
{
    public class RiskAssessmentService : IRiskAssessmentService
    {
        private readonly IRiskAssessmentRepository _riskAssessmentRepository;
        private readonly VendorRiskScoreDbContext _context;

        public RiskAssessmentService(IRiskAssessmentRepository riskAssessmentRepository, VendorRiskScoreDbContext context)
        {
            _riskAssessmentRepository = riskAssessmentRepository;
            _context = context;
        }

        public async Task<List<RiskAssessment>> GetAllRiskAssessmentsAsync()
        {
            List<RiskAssessment> riskAssessments = await _riskAssessmentRepository.GetAllRiskAssessmentsAsync();
            return riskAssessments;
        }

        public async Task<RiskAssessment> GetRiskAssessmentByIdAsync(int id)
        {
            RiskAssessment riskAssessment = await _riskAssessmentRepository.GetRiskAssessmentByIdAsync(id);

            if(riskAssessment == null)
            {
                throw new KeyNotFoundException($"There isn't riskAssessment belong with this id:{id}");
            }

            return riskAssessment;
        }

        public async Task AddRiskAssessmentAsync(RiskAssessment riskAssessment)
        {
            if(riskAssessment == null)
            {
                throw new ArgumentNullException("RiskAssessment cannot be null");
            }

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _riskAssessmentRepository.AddRiskAssessmentAsync(riskAssessment);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateRiskAssessmentAsync(int id, RiskAssessment riskAssessment)
        {
            if(riskAssessment == null)
            {
                throw new ArgumentNullException("RiskAssessment cannot be null");
            }

            RiskAssessment dbRiskAssesment = await GetRiskAssessmentByIdAsync(id);

            if(!String.IsNullOrEmpty(riskAssessment.RiskLevel) && riskAssessment.RiskLevel != dbRiskAssesment.RiskLevel)
            {
                dbRiskAssesment.RiskLevel = riskAssessment.RiskLevel;
            }

            if(riskAssessment.RiskScore != dbRiskAssesment.RiskScore && riskAssessment.RiskScore >= 0)
            {
                dbRiskAssesment.RiskScore = riskAssessment.RiskScore;
            }

            if(riskAssessment.VendorId != dbRiskAssesment.VendorId)
            {
                dbRiskAssesment.VendorId = riskAssessment.VendorId;
            }

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _riskAssessmentRepository.UpdateRiskAssessmentAsync(dbRiskAssesment);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task DeleteRiskAssessmentAsync(int id)
        {
            RiskAssessment dbRiskAssesment = await GetRiskAssessmentByIdAsync(id);

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _riskAssessmentRepository.DeleteRiskAssessmentAsync(dbRiskAssesment);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
