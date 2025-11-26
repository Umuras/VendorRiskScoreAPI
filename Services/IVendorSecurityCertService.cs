using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Services
{
    public interface IVendorSecurityCertService
    {
        Task<List<VendorSecurityCert>> GetAllVendorSecurityCertsAsync();
        Task<VendorSecurityCert> GetVendorSecurityCertByIdAsync(int id);
        Task AddVendorSecurityCertAsync(VendorSecurityCert vendorSecurityCert);
        Task UpdateVendorSecurityCertAsync(int id, VendorSecurityCert vendorSecurityCert);
        Task DeleteVendorSecurityCertAsync(int id);
    }
}
