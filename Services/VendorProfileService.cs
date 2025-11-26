using Microsoft.EntityFrameworkCore.Storage;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Repositories;

namespace VendorRiskScoreAPI.Services
{
    public class VendorProfileService : IVendorProfileService
    {
        private readonly IVendorProfileRepository _vendorProfileRepository;
        private readonly VendorRiskScoreDbContext _context;

        public VendorProfileService(IVendorProfileRepository vendorProfileRepository, VendorRiskScoreDbContext context)
        {
            _vendorProfileRepository = vendorProfileRepository;
            _context = context;
        }

        public async Task<List<VendorProfile>> GetVendorProfilesAsync()
        {
            List<VendorProfile> vendorProfiles = await _vendorProfileRepository.GetVendorProfilesAsync();
            return vendorProfiles;
        }

        public async Task<VendorProfile> GetVendorProfileByIdAsync(int id)
        {
            VendorProfile vendorProfile = await _vendorProfileRepository.GetVendorProfileByIdAsync(id);
            
            if(vendorProfile == null)
            {
                throw new KeyNotFoundException($"There isn't vendorProfile belong with this id:{id}");
            }

            return vendorProfile;
        }

        public async Task<VendorProfile> AddVendorProfileAsync(VendorProfile vendorProfile)
        {
            if(vendorProfile == null)
            {
                throw new ArgumentNullException("VendorProfile cannot be null");
            }

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _vendorProfileRepository.AddVendorProfileAsync(vendorProfile);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return vendorProfile;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateVendorProfileAsync(int id, VendorProfile vendorProfile)
        {
            if (vendorProfile == null)
            {
                throw new ArgumentNullException("VendorProfile cannot be null");
            }

            VendorProfile dbVendorProfile = await GetVendorProfileByIdAsync(id);

            if(vendorProfile.Name != null)
            {
                dbVendorProfile.Name = vendorProfile.Name;
            }

            if(vendorProfile.SecurityCerts != null)
            {
                dbVendorProfile.SecurityCerts = vendorProfile.SecurityCerts;
            }

            if(vendorProfile.FinancialHealth != dbVendorProfile.FinancialHealth && vendorProfile.FinancialHealth > 0)
            {
                dbVendorProfile.FinancialHealth = vendorProfile.FinancialHealth;
            }

            if(vendorProfile.SlaUpTime != dbVendorProfile.SlaUpTime && vendorProfile.SlaUpTime > 0)
            {
                dbVendorProfile.SlaUpTime = vendorProfile.SlaUpTime;
            }

            if(vendorProfile.MajorIncidents != dbVendorProfile.MajorIncidents && vendorProfile.MajorIncidents > 0)
            {
                dbVendorProfile.MajorIncidents = vendorProfile.MajorIncidents;
            }

            if(vendorProfile.Document != null)
            {
                dbVendorProfile.Document = vendorProfile.Document;
            }

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _vendorProfileRepository.UpdateVendorProfileAsync(dbVendorProfile);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteVendorProfile(int id)
        {
            VendorProfile dbVendorProfile = await GetVendorProfileByIdAsync(id);

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _vendorProfileRepository.DeleteVendorProfile(dbVendorProfile);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
