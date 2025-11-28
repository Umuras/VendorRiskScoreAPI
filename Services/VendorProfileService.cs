using Microsoft.EntityFrameworkCore.Storage;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Dtos;
using VendorRiskScoreAPI.Repositories;

namespace VendorRiskScoreAPI.Services
{
    public class VendorProfileService : IVendorProfileService
    {
        private readonly IVendorProfileRepository _vendorProfileRepository;
        private readonly VendorRiskScoreDbContext _context;
        private readonly IRuleEngineService _ruleEngineService;
        private readonly IRiskAssessmentService _riskAssessmentService;
        private readonly IDocumentService _documentService;

        public VendorProfileService(IVendorProfileRepository vendorProfileRepository, VendorRiskScoreDbContext context,
            IRuleEngineService ruleEngineService, IRiskAssessmentService riskAssessmentService, IDocumentService documentService)
        {
            _vendorProfileRepository = vendorProfileRepository;
            _context = context;
            _ruleEngineService = ruleEngineService;
            _riskAssessmentService = riskAssessmentService;
            _documentService = documentService;
        }

        public async Task<List<VendorProfile>> GetVendorProfilesAsync()
        {
            List<VendorProfile> vendorProfiles = await _vendorProfileRepository.GetVendorProfilesAsync();
            return vendorProfiles;
        }

        public async Task<VendorProfile> GetVendorProfileByIdAsync(int id)
        {
            VendorProfile vendorProfile = await _vendorProfileRepository.GetVendorProfileByIdAsync(id);
            
            if(vendorProfile == null)
            {
                throw new KeyNotFoundException($"There isn't vendorProfile belong with this id:{id}");
            }

            return vendorProfile;
        }

        public async Task<VendorProfile> AddVendorProfileAsync(VendorProfileDto vendorProfileRequest)
        {
            if(vendorProfileRequest == null)
            {
                throw new ArgumentNullException("VendorProfile cannot be null");
            }

            Document document = new Document();
            document.ContractValid = vendorProfileRequest.Documents.ContractValid;
            document.PrivacyPolicyValid = vendorProfileRequest.Documents.PrivacyPolicyValid;
            document.PentestReportValid = vendorProfileRequest.Documents.PentestReportValid;

            List<VendorSecurityCert> vendorSecurityCerts = new List<VendorSecurityCert>();
            foreach (string certifacateName in vendorProfileRequest.SecurityCerts)
            {
                vendorSecurityCerts.Add(new VendorSecurityCert() { CertName = certifacateName});
            }


            RiskAssessment riskAssessment = new RiskAssessment();
            double finalScore = _riskAssessmentService.CalculateFinalScore(vendorProfileRequest.FinancialHealth, vendorProfileRequest.SlaUptime, vendorProfileRequest.MajorIncidents,
                vendorSecurityCerts, document);

            riskAssessment.RiskScore = (float)finalScore;
            riskAssessment.RiskLevel = _riskAssessmentService.CalculateRiskLevel(finalScore);

            VendorProfile vendorProfile = new VendorProfile()
            {
                Name = vendorProfileRequest.Name,
                FinancialHealth = vendorProfileRequest.FinancialHealth,
                MajorIncidents = vendorProfileRequest.MajorIncidents,
                SlaUpTime = vendorProfileRequest.SlaUptime,
                Document = document,
                SecurityCerts = vendorSecurityCerts,
                RiskAssessment = riskAssessment
            };

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _vendorProfileRepository.AddVendorProfileAsync(vendorProfile);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return vendorProfile;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateVendorProfileAsync(int id, VendorProfileDto vendorProfileRequest)
        {
            if (vendorProfileRequest == null)
            {
                throw new ArgumentNullException("VendorProfile cannot be null");
            }

            List<VendorSecurityCert> vendorSecurityCerts = new List<VendorSecurityCert>();
            foreach (string certifacateName in vendorProfileRequest.SecurityCerts)
            {
                vendorSecurityCerts.Add(new VendorSecurityCert() { CertName = certifacateName });
            }

            VendorProfile dbVendorProfile = await GetVendorProfileByIdAsync(id);

            Document dbDocument = await _documentService.GetDocumentByIdAsync(dbVendorProfile.Document.Id);
            dbDocument.ContractValid = vendorProfileRequest.Documents.ContractValid;
            dbDocument.PrivacyPolicyValid = vendorProfileRequest.Documents.PrivacyPolicyValid;
            dbDocument.PentestReportValid = vendorProfileRequest.Documents.PentestReportValid;


            if (vendorProfileRequest.Name != null)
            {
                dbVendorProfile.Name = vendorProfileRequest.Name;
            }

            if(vendorProfileRequest.SecurityCerts != null)
            {
                dbVendorProfile.SecurityCerts = vendorSecurityCerts;
            }

            if(vendorProfileRequest.FinancialHealth != dbVendorProfile.FinancialHealth && vendorProfileRequest.FinancialHealth > 0)
            {
                dbVendorProfile.FinancialHealth = vendorProfileRequest.FinancialHealth;
            }

            if(vendorProfileRequest.SlaUptime != dbVendorProfile.SlaUpTime && vendorProfileRequest.SlaUptime > 0)
            {
                dbVendorProfile.SlaUpTime = vendorProfileRequest.SlaUptime;
            }

            if(vendorProfileRequest.MajorIncidents != dbVendorProfile.MajorIncidents && vendorProfileRequest.MajorIncidents >= 0)
            {
                dbVendorProfile.MajorIncidents = vendorProfileRequest.MajorIncidents;
            }

            if(vendorProfileRequest.Documents != null)
            {
                dbVendorProfile.Document = dbDocument;
            }

            RiskAssessment dbRiskAssessment = await _riskAssessmentService.GetRiskAssessmentByIdAsync(dbVendorProfile.RiskAssessment.Id);
            double finalScore = _riskAssessmentService.CalculateFinalScore(vendorProfileRequest.FinancialHealth, vendorProfileRequest.SlaUptime, vendorProfileRequest.MajorIncidents,
                vendorSecurityCerts, dbDocument);

            dbRiskAssessment.RiskScore = (float)finalScore;
            dbRiskAssessment.RiskLevel = _riskAssessmentService.CalculateRiskLevel(finalScore);

            dbVendorProfile.RiskAssessment = dbRiskAssessment;

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _vendorProfileRepository.UpdateVendorProfileAsync(dbVendorProfile);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteVendorProfile(int id)
        {
            VendorProfile dbVendorProfile = await GetVendorProfileByIdAsync(id);

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _vendorProfileRepository.DeleteVendorProfile(dbVendorProfile);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public VendorProfileRiskScoreResponseDto CreateVendorProfileRiskScoreResponseDto(VendorProfile vendorProfile)
        {
            List<VendorSecurityCert> vendorSecurityCerts = new List<VendorSecurityCert>();
            foreach (VendorSecurityCert item in vendorProfile.SecurityCerts)
            {
                vendorSecurityCerts.Add(new VendorSecurityCert() { CertName = item.CertName });
            }

            VendorProfileRiskScoreResponseDto vendorProfileRiskScoreResponseDto = new VendorProfileRiskScoreResponseDto()
            {
                Vendor = vendorProfile.Name,
                Financial = _ruleEngineService.CalculateFinancialRisk(vendorProfile.FinancialHealth),
                Operational = _ruleEngineService.CalculateOperationalRisk(vendorProfile.SlaUpTime, vendorProfile.MajorIncidents),
                Security = _ruleEngineService.CalculateSecurityComplianceRisk(vendorSecurityCerts, vendorProfile.Document)
            };

            vendorProfileRiskScoreResponseDto.FinalScore = (vendorProfileRiskScoreResponseDto.Financial * 0.4) + (vendorProfileRiskScoreResponseDto.Operational * 0.3) +
                (vendorProfileRiskScoreResponseDto.Security * 0.3);

            return vendorProfileRiskScoreResponseDto;
        }

        public VendorProfileDto CreateVendorProfileDto(VendorProfile vendorProfile)
        {
            List<string> securityCerts = new List<string>();
            foreach (VendorSecurityCert cert in vendorProfile.SecurityCerts)
            {
                securityCerts.Add(cert.CertName);
            }

            DocumentDto documentDto = new DocumentDto()
            {
                ContractValid = vendorProfile.Document.ContractValid,
                PrivacyPolicyValid = vendorProfile.Document.PrivacyPolicyValid,
                PentestReportValid = vendorProfile.Document.PentestReportValid
            };

            VendorProfileDto vendorProfileDto = new VendorProfileDto()
            {
                Name = vendorProfile.Name,
                FinancialHealth = vendorProfile.FinancialHealth,
                MajorIncidents = vendorProfile.MajorIncidents,
                SlaUptime = vendorProfile.SlaUpTime,
                Documents = documentDto,
                SecurityCerts = securityCerts
            };

            return vendorProfileDto;
        }
    }
}
