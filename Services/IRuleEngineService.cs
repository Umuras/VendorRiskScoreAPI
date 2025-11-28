using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Services
{
    public interface IRuleEngineService
    {
        float CalculateFinancialRisk(int financialHealth);
        float CalculateOperationalRisk(int slaUpTime, int majorIncidents);
        float CalculateSecurityComplianceRisk(List<VendorSecurityCert> vendorSecurityCerts, Document document);
        Task<string> GetReasons(int slaUpTime, List<VendorSecurityCert> vendorSecurityCerts, Document document);
    }
}
