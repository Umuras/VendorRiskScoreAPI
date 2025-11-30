using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Dtos;

namespace VendorRiskScoreAPI.Services
{
    public interface IVendorProfileService
    {
        Task<List<VendorProfile>> GetVendorProfilesAsync();
        Task<VendorProfile> GetVendorProfileByIdAsync(int id);
        Task<VendorProfile> AddVendorProfileAsync(VendorProfileRequestDto vendorProfile);
        Task UpdateVendorProfileAsync(int id, VendorProfileRequestDto vendorProfile);
        Task DeleteVendorProfile(int id);
        VendorProfileRiskScoreResponseDto CreateVendorProfileRiskScoreResponseDto(VendorProfile vendorProfile);
        VendorProfileResponseDto CreateVendorProfileDto(VendorProfile vendorProfile);
    }
}
