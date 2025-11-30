using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Dtos;

namespace VendorRiskScoreAPI.Services
{
    public interface IVendorProfileRiskScoreService
    {
        Task<List<VendorProfileRiskScore>> GetVendorProfilesRiskScoresAsync();
        Task<VendorProfileRiskScore> GetVendorProfileRiskScoreByIdAsync(int id);
        Task<VendorProfileRiskScore> AddVendorProfileRiskScoreAsync(VendorProfileRiskScore vendorProfileRiskScore);
        Task UpdateVendorProfileRiskScoreAsync(int id, VendorProfileRiskScore vendorProfileRiskScore);
        Task DeleteVendorProfileRiskScore(int id);
        Task<VendorProfileRiskScoreResponseDto> CreateVendorProfileRiskScoreResponseDto(VendorProfile vendorProfile);
    }
}
