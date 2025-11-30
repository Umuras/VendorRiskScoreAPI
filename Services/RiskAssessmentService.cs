using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection.Metadata;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Domain.ValueObjects;
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

        public decimal CalculateFinalScore(int financialHealth, int slaUpTime, int majorIncidents,
           List<VendorSecurityCert> vendorSecurityCerts, Domain.Entities.Document document)
        {
            decimal finalScore = (_ruleEngineService.CalculateFinancialRisk(financialHealth) * 0.4m) +
                (_ruleEngineService.CalculateOperationalRisk(slaUpTime, majorIncidents) * 0.3m) +
                (_ruleEngineService.CalculateSecurityComplianceRisk(vendorSecurityCerts, document) * 0.3m);
            return finalScore;
        }

        public string CalculateRiskLevel(decimal riskScore)
        {
            if (riskScore >= 0.00m && riskScore <= 0.33m)
            {
                return "Low";
            }
            else if (riskScore >= 0.34m && riskScore <= 0.70m)
            {
                return "Medium";
            }
            else if(riskScore >= 0.71m && riskScore <= 1.00m)
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

        public Task<RiskScores> CalculateVendorProfileRiskScores(string vendorProfileName, int financialHealth, int slaUpTime, int majorIncidents,
           List<VendorSecurityCert> vendorSecurityCerts, Domain.Entities.Document document)
        {
            decimal financialRisk = (_ruleEngineService.CalculateFinancialRisk(financialHealth) * 0.4m);
            decimal operationalRisk = (_ruleEngineService.CalculateOperationalRisk(slaUpTime, majorIncidents) * 0.3m);
            decimal securityRisk = (_ruleEngineService.CalculateSecurityComplianceRisk(vendorSecurityCerts, document) * 0.3m);
            decimal finalScore = CalculateFinalScore(financialHealth, slaUpTime, majorIncidents, vendorSecurityCerts, document);

            RiskScores riskScores = new RiskScores()
            {
                Financial = financialRisk,
                Operational = operationalRisk,
                Security = securityRisk,
                FinalScore = finalScore
            };

            return Task.FromResult(riskScores);
        }


    }
}
