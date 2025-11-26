using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Services
{
    public interface IVendorProfileService
    {
        Task<List<VendorProfile>> GetVendorProfilesAsync();
        Task<VendorProfile> GetVendorProfileByIdAsync(int id);
        Task<VendorProfile> AddVendorProfileAsync(VendorProfile vendorProfile);
        Task UpdateVendorProfileAsync(int id, VendorProfile vendorProfile);
        Task DeleteVendorProfile(int id);
    }
}
