using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection.Metadata;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Domain.ValueObjects;
using VendorRiskScoreAPI.Dtos;
using VendorRiskScoreAPI.Enums;
using VendorRiskScoreAPI.Exceptions;
using VendorRiskScoreAPI.Repositories;
using Document = VendorRiskScoreAPI.Domain.Entities.Document;

namespace VendorRiskScoreAPI.Services
{
    public class VendorProfileService : IVendorProfileService
    {
        private readonly IVendorProfileRepository _vendorProfileRepository;
        private readonly VendorRiskScoreDbContext _context;
        private readonly IRuleEngineService _ruleEngineService;
        private readonly IRiskAssessmentService _riskAssessmentService;
        private readonly IDocumentService _documentService;
        private readonly IVendorProfileRiskScoreService _vendorProfileRiskScoreService;
        private HashSet<string> allowedCertifacates = new HashSet<string>();

        public VendorProfileService(IVendorProfileRepository vendorProfileRepository, VendorRiskScoreDbContext context,
            IRuleEngineService ruleEngineService, IRiskAssessmentService riskAssessmentService, IDocumentService documentService,
            IVendorProfileRiskScoreService vendorProfileRiskScoreService)
        {
            _vendorProfileRepository = vendorProfileRepository;
            _context = context;
            _ruleEngineService = ruleEngineService;
            _riskAssessmentService = riskAssessmentService;
            _documentService = documentService;
            _vendorProfileRiskScoreService = vendorProfileRiskScoreService;
            Init();
        }

        private void Init()
        {
            allowedCertifacates.Add(SecurityCertificates.ISO27001.ToString());
            allowedCertifacates.Add(SecurityCertificates.SOC2.ToString());
            allowedCertifacates.Add(SecurityCertificates.PCI_DSS.ToString());
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

        public async Task<VendorProfile> AddVendorProfileAsync(VendorProfileRequestDto vendorProfileRequest)
        {
            if(vendorProfileRequest == null)
            {
                throw new ArgumentNullException("VendorProfile cannot be null");
            }

            await VendorProfileSameNameControl(vendorProfileRequest.Name);

            Document document = new Document();
            document.ContractValid = vendorProfileRequest.Document.ContractValid;
            document.PrivacyPolicyValid = vendorProfileRequest.Document.PrivacyPolicyValid;
            document.PentestReportValid = vendorProfileRequest.Document.PentestReportValid;

            List<VendorSecurityCert> vendorSecurityCerts = new List<VendorSecurityCert>();
            vendorSecurityCerts = CheckVendorProfileSecurityCertificates(vendorProfileRequest);

            RiskAssessment riskAssessment = new RiskAssessment();
            decimal finalScore = _riskAssessmentService.CalculateFinalScore(vendorProfileRequest.FinancialHealth, vendorProfileRequest.SlaUptime, vendorProfileRequest.MajorIncidents,
                vendorSecurityCerts, document);

            riskAssessment.RiskScore = finalScore;
            riskAssessment.RiskLevel = _riskAssessmentService.CalculateRiskLevel(finalScore);

            RiskScores riskScores = await _riskAssessmentService.CalculateVendorProfileRiskScores(vendorProfileRequest.Name, 
                vendorProfileRequest.FinancialHealth, vendorProfileRequest.SlaUptime, vendorProfileRequest.MajorIncidents,
                vendorSecurityCerts, document);

            VendorProfileRiskScore vendorProfileRiskScore = new VendorProfileRiskScore()
            {
                VendorName = vendorProfileRequest.Name,
                Financial = riskScores.Financial,
                Operational = riskScores.Operational,
                Security = riskScores.Security,
                FinalScore = riskScores.FinalScore
            };

            VendorProfile vendorProfile = new VendorProfile()
            {
                Name = vendorProfileRequest.Name,
                FinancialHealth = vendorProfileRequest.FinancialHealth,
                MajorIncidents = vendorProfileRequest.MajorIncidents,
                SlaUpTime = vendorProfileRequest.SlaUptime,
                Document = document,
                SecurityCerts = vendorSecurityCerts,
                RiskAssessment = riskAssessment,
                VendorProfileRiskScore = vendorProfileRiskScore
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

        public async Task UpdateVendorProfileAsync(int id, VendorProfileRequestDto vendorProfileRequest)
        {
            if (vendorProfileRequest == null)
            {
                throw new ArgumentNullException("VendorProfile cannot be null");
            }

            List<VendorSecurityCert> vendorSecurityCerts = new List<VendorSecurityCert>();
            vendorSecurityCerts = CheckVendorProfileSecurityCertificates(vendorProfileRequest);

            VendorProfile dbVendorProfile = await GetVendorProfileByIdAsync(id);

            Domain.Entities.Document dbDocument = await _documentService.GetDocumentByIdAsync(dbVendorProfile.Document.Id);
            dbDocument.ContractValid = vendorProfileRequest.Document.ContractValid;
            dbDocument.PrivacyPolicyValid = vendorProfileRequest.Document.PrivacyPolicyValid;
            dbDocument.PentestReportValid = vendorProfileRequest.Document.PentestReportValid;


            if (vendorProfileRequest.Name != null && vendorProfileRequest.Name != dbVendorProfile.Name)
            {
                await VendorProfileSameNameControl(vendorProfileRequest.Name);
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

            if(vendorProfileRequest.Document != null)
            {
                dbVendorProfile.Document = dbDocument;
            }

            RiskAssessment dbRiskAssessment = await _riskAssessmentService.GetRiskAssessmentByIdAsync(dbVendorProfile.RiskAssessment.Id);
            decimal finalScore = _riskAssessmentService.CalculateFinalScore(vendorProfileRequest.FinancialHealth, vendorProfileRequest.SlaUptime, vendorProfileRequest.MajorIncidents,
                vendorSecurityCerts, dbDocument);

            dbRiskAssessment.RiskScore = finalScore;
            dbRiskAssessment.RiskLevel = _riskAssessmentService.CalculateRiskLevel(finalScore);

            dbVendorProfile.RiskAssessment = dbRiskAssessment;

            VendorProfileRiskScore dbVendorProfileRiskScore = await _vendorProfileRiskScoreService.GetVendorProfileRiskScoreByIdAsync(dbVendorProfile.VendorProfileRiskScore.Id);
            RiskScores riskScores = await _riskAssessmentService.CalculateVendorProfileRiskScores(vendorProfileRequest.Name,
                vendorProfileRequest.FinancialHealth, vendorProfileRequest.SlaUptime, vendorProfileRequest.MajorIncidents,
                vendorSecurityCerts, dbDocument);

            dbVendorProfileRiskScore.Financial = riskScores.Financial;
            dbVendorProfileRiskScore.Operational = riskScores.Operational;
            dbVendorProfileRiskScore.Security = riskScores.Security;
            dbVendorProfileRiskScore.FinalScore = riskScores.FinalScore;

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
                Security = _ruleEngineService.CalculateSecurityComplianceRisk(vendorSecurityCerts, vendorProfile.Document),
                FinalScore = _riskAssessmentService.CalculateFinalScore(vendorProfile.FinancialHealth, vendorProfile.SlaUpTime,
                vendorProfile.MajorIncidents, vendorSecurityCerts, vendorProfile.Document)
            };

            return vendorProfileRiskScoreResponseDto;
        }

        public VendorProfileResponseDto CreateVendorProfileDto(VendorProfile vendorProfile)
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

            VendorProfileResponseDto vendorProfileResponseDto = new VendorProfileResponseDto()
            {
                Id = vendorProfile.Id,
                Name = vendorProfile.Name,
                FinancialHealth = vendorProfile.FinancialHealth,
                MajorIncidents = vendorProfile.MajorIncidents,
                SlaUptime = vendorProfile.SlaUpTime,
                Documents = documentDto,
                SecurityCerts = securityCerts
            };

            return vendorProfileResponseDto;
        }

        private async Task VendorProfileSameNameControl(string name)
        {
            bool vendorProfileExistWithSameName = await _vendorProfileRepository.VendorProfileExistsByNameAsync(name);
            if (vendorProfileExistWithSameName)
            {
                throw new DuplicateVendorProfileNameException($"{name}");
            }
        }

        private List<VendorSecurityCert> CheckVendorProfileSecurityCertificates(VendorProfileRequestDto vendorProfileRequest)
        {
            List<VendorSecurityCert> vendorSecurityCerts = new List<VendorSecurityCert>();
            vendorProfileRequest.SecurityCerts = vendorProfileRequest.SecurityCerts.
                Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();

            if (vendorProfileRequest.SecurityCerts.Count() > 3)
            {
                throw new ArgumentException($"You can only add 3 quantities certifacates");
            }

            foreach (string certifacateName in vendorProfileRequest.SecurityCerts)
            {
                if (certifacateName != string.Empty)
                {
                    if (!allowedCertifacates.Contains(certifacateName))
                    {
                        throw new ArgumentException($"You can only add this 3 certifacates: {SecurityCertificates.ISO27001.ToString()}, " +
                            $"{SecurityCertificates.PCI_DSS.ToString()}, {SecurityCertificates.SOC2.ToString()}");
                    }
                }
                else
                {
                    continue;
                }

                vendorSecurityCerts.Add(new VendorSecurityCert() { CertName = certifacateName });
            }

            return vendorSecurityCerts;
        }
    }
}
