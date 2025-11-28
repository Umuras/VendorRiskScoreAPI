using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Dtos;

namespace VendorRiskScoreAPI.Services
{
    public interface IVendorProfileService
    {
        Task<List<VendorProfile>> GetVendorProfilesAsync();
        Task<VendorProfile> GetVendorProfileByIdAsync(int id);
        Task<VendorProfile> AddVendorProfileAsync(VendorProfileDto vendorProfile);
        Task UpdateVendorProfileAsync(int id, VendorProfileDto vendorProfile);
        Task DeleteVendorProfile(int id);
        VendorProfileRiskScoreResponseDto CreateVendorProfileRiskScoreResponseDto(VendorProfile vendorProfile);
        VendorProfileDto CreateVendorProfileDto(VendorProfile vendorProfile);
    }
}
