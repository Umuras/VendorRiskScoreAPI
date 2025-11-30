
using Microsoft.EntityFrameworkCore;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Repositories
{
    public class VendorProfileRiskScoreRepository : IVendorProfileRiskScoreRepository
    {
        private readonly VendorRiskScoreDbContext _context;

        public VendorProfileRiskScoreRepository(VendorRiskScoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<VendorProfileRiskScore>> GetVendorProfilesRiskScoresAsync()
        {
            return await _context.VendorProfileRiskScores.ToListAsync();
        }

        public async Task<VendorProfileRiskScore> GetVendorProfileRiskScoreByIdAsync(int id)
        {
            VendorProfileRiskScore? vendorProfileRiskScore = await _context.VendorProfileRiskScores.FirstOrDefaultAsync(vprs => vprs.Id == id);
            return vendorProfileRiskScore;
        }

        public async Task<VendorProfileRiskScore> AddVendorProfileRiskScoreAsync(VendorProfileRiskScore vendorProfileRiskScore)
        {
            await _context.VendorProfileRiskScores.AddAsync(vendorProfileRiskScore);
            return vendorProfileRiskScore;
        }

        public Task UpdateVendorProfileRiskScoreAsync(VendorProfileRiskScore vendorProfileRiskScore)
        {
            _context.VendorProfileRiskScores.Update(vendorProfileRiskScore);
            return Task.CompletedTask;
        }

        public Task DeleteVendorProfileRiskScore(VendorProfileRiskScore vendorProfileRiskScore)
        {
            _context.VendorProfileRiskScores.Remove(vendorProfileRiskScore);
            return Task.CompletedTask;
        }
    }
}
