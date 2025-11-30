using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Repositories
{
    public interface IVendorProfileRiskScoreRepository
    {
        Task<List<VendorProfileRiskScore>> GetVendorProfilesRiskScoresAsync();
        Task<VendorProfileRiskScore> GetVendorProfileRiskScoreByIdAsync(int id);
        Task<VendorProfileRiskScore> AddVendorProfileRiskScoreAsync(VendorProfileRiskScore vendorProfileRiskScore);
        Task UpdateVendorProfileRiskScoreAsync(VendorProfileRiskScore vendorProfileRiskScore);
        Task DeleteVendorProfileRiskScore(VendorProfileRiskScore vendorProfileRiskScore);
    }
}
