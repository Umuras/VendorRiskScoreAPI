using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Repositories
{
    public interface IVendorProfileRepository
    {
        Task<List<VendorProfile>> GetVendorProfilesAsync();
        Task<VendorProfile> GetVendorProfileByIdAsync(int id);
        Task<VendorProfile> AddVendorProfileAsync(VendorProfile vendorProfile);
        Task UpdateVendorProfileAsync(VendorProfile vendorProfile);
        Task DeleteVendorProfile(VendorProfile vendorProfile);
        Task<bool> VendorProfileExistsByNameAsync(string name);
    }
}
