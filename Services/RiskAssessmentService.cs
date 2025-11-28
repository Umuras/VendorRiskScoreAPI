using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection.Metadata;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Dtos;
using VendorRiskScoreAPI.Repositories;

namespace VendorRiskScoreAPI.Services
{
    public class RiskAssessmentService : IRiskAssessmentService
    {
        private readonly IRiskAssessmentRepository _riskAssessmentRepository;
        private readonly IRuleEngineService _ruleEngineService;
        private readonly VendorRiskScoreDbContext _context;

        public RiskAssessmentService(IRiskAssessmentRepository riskAssessmentRepository, IRuleEngineService ruleEngineService, VendorRiskScoreDbContext context)
        {
            _riskAssessmentRepository = riskAssessmentRepository;
            _ruleEngineService = ruleEngineService;
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

        public double CalculateFinalScore(int financialHealth, int slaUpTime, int majorIncidents,
           List<VendorSecurityCert> vendorSecurityCerts, Domain.Entities.Document document)
        {
            double finalScore = (_ruleEngineService.CalculateFinancialRisk(financialHealth) * 0.4) +
                (_ruleEngineService.CalculateOperationalRisk(slaUpTime, majorIncidents) * 0.3) +
                (_ruleEngineService.CalculateSecurityComplianceRisk(vendorSecurityCerts, document) * 0.3);
            return finalScore;
        }

        public string CalculateRiskLevel(double riskScore)
        {
            if (riskScore >= 0.00 && riskScore <= 0.33)
            {
                return "Low";
            }
            else if (riskScore >= 0.34 && riskScore <= 0.70)
            {
                return "Medium";
            }
            else if(riskScore >= 0.71 && riskScore <= 1.00)
            {
                return "High";
            }else
            {
                return "Critic";
            }
        }

        public async Task<RiskAssessmentResponseDto> CreateRiskAssessmentResult(VendorProfile vendorProfile)
        {
            List<VendorSecurityCert> vendorSecurityCerts = new List<VendorSecurityCert>();
            foreach (var item in vendorProfile.SecurityCerts)
            {
                vendorSecurityCerts.Add(new VendorSecurityCert { CertName = item.CertName });
            }

            string reason = await _ruleEngineService.GetReasons(vendorProfile.SlaUpTime, vendorSecurityCerts, vendorProfile.Document);

            RiskAssessmentResponseDto riskAssessmentResponseDto = new RiskAssessmentResponseDto()
            {
                RiskScore = vendorProfile.RiskAssessment.RiskScore,
                RiskLevel = vendorProfile.RiskAssessment.RiskLevel,
                Reason = reason
            };

            return riskAssessmentResponseDto;
        }
    }
}
