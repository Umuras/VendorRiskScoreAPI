using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Services
{
    public interface IRuleEngineService
    {
        decimal CalculateFinancialRisk(int financialHealth);
        decimal CalculateOperationalRisk(int slaUpTime, int majorIncidents);
        decimal CalculateSecurityComplianceRisk(List<VendorSecurityCert> vendorSecurityCerts, Document document);
        Task<string> GetReasons(int slaUpTime, List<VendorSecurityCert> vendorSecurityCerts, Document document);
    }
}
