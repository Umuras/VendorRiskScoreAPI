using Microsoft.EntityFrameworkCore;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Repositories
{
    public class VendorSecurityCertRepository : IVendorSecurityCertRepository
    {
        private readonly VendorRiskScoreDbContext _context;

        public VendorSecurityCertRepository(VendorRiskScoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<VendorSecurityCert>> GetAllVendorSecurityCertsAsync()
        {
            return await _context.VendorSecurityCerts.ToListAsync();
        }

        public async Task<VendorSecurityCert> GetVendorSecurityCertByIdAsync(int id)
        {
            VendorSecurityCert? vendorSecurityCert = await _context.VendorSecurityCerts.FirstOrDefaultAsync(vs => vs.Id == id);
            return vendorSecurityCert;
        }

        public async Task AddVendorSecurityCertAsync(VendorSecurityCert vendorSecurityCert)
        {
            await _context.VendorSecurityCerts.AddAsync(vendorSecurityCert);
        }

        public Task UpdateVendorSecurityCertAsync(VendorSecurityCert vendorSecurityCert)
        {
            _context.VendorSecurityCerts.Update(vendorSecurityCert);
            return Task.CompletedTask;
        }

        public Task DeleteVendorSecurityCertAsync(VendorSecurityCert vendorSecurityCert)
        {
            _context.VendorSecurityCerts.Remove(vendorSecurityCert);
            return Task.CompletedTask;
        }
    }
}
