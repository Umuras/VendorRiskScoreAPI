using Microsoft.EntityFrameworkCore;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Repositories
{
    public class VendorProfileRepository : IVendorProfileRepository
    {
        private readonly VendorRiskScoreDbContext _context;

        public VendorProfileRepository(VendorRiskScoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<VendorProfile>> GetVendorProfilesAsync()
        {
            return await _context.VendorProfiles.Include(v => v.Document).Include(v => v.SecurityCerts).Include(v => v.RiskAssessment).ToListAsync();
        }

        public async Task<VendorProfile> GetVendorProfileByIdAsync(int id)
        {
            VendorProfile? vendorProfile = await _context.VendorProfiles.Include(v => v.Document).Include(v => v.SecurityCerts).Include(v => v.RiskAssessment).FirstOrDefaultAsync(v => v.Id == id);
            return vendorProfile;
        }

        public async Task<VendorProfile> AddVendorProfileAsync(VendorProfile vendorProfile)
        {
            await _context.VendorProfiles.AddAsync(vendorProfile);
            return vendorProfile;
        }

        public Task UpdateVendorProfileAsync(VendorProfile vendorProfile)
        {
            _context.VendorProfiles.Update(vendorProfile);
            return Task.CompletedTask;
        }

        public Task DeleteVendorProfile(VendorProfile vendorProfile)
        {
            _context.VendorProfiles.Remove(vendorProfile);
            return Task.CompletedTask;
        }
    }
}
