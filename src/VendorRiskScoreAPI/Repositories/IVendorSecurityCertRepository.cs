using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Repositories
{
    public interface IVendorSecurityCertRepository
    {
        Task<List<VendorSecurityCert>> GetAllVendorSecurityCertsAsync();
        Task<VendorSecurityCert> GetVendorSecurityCertByIdAsync(int id);
        Task AddVendorSecurityCertAsync(VendorSecurityCert vendorSecurityCert);
        Task UpdateVendorSecurityCertAsync(VendorSecurityCert vendorSecurityCert);
        Task DeleteVendorSecurityCertAsync(VendorSecurityCert vendorSecurityCert);
    }
}
