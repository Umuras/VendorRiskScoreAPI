using Microsoft.EntityFrameworkCore.Storage;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Repositories;

namespace VendorRiskScoreAPI.Services
{
    public class VendorSecurityCertService : IVendorSecurityCertService
    {
        private readonly IVendorSecurityCertRepository _vendorSecurityCertRepository;
        private readonly VendorRiskScoreDbContext _context;

        public VendorSecurityCertService(IVendorSecurityCertRepository vendorSecurityCertRepository, VendorRiskScoreDbContext context)
        {
            _vendorSecurityCertRepository = vendorSecurityCertRepository;
            _context = context;
        }

        public async Task<List<VendorSecurityCert>> GetAllVendorSecurityCertsAsync()
        {
            List<VendorSecurityCert> vendorSecurityCerts = await _vendorSecurityCertRepository.GetAllVendorSecurityCertsAsync();
            return vendorSecurityCerts;
        }

        public async Task<VendorSecurityCert> GetVendorSecurityCertByIdAsync(int id)
        {
            VendorSecurityCert vendorSecurityCert = await _vendorSecurityCertRepository.GetVendorSecurityCertByIdAsync(id);
            
            if(vendorSecurityCert == null)
            {
                throw new KeyNotFoundException($"There isn't vendorSecurityCert belong with this id:{id}");
            }

            return vendorSecurityCert;
        }

        public async Task AddVendorSecurityCertAsync(VendorSecurityCert vendorSecurityCert)
        {
            if(vendorSecurityCert == null)
            {
                throw new ArgumentNullException("VendorSecurityCert cannot be null");
            }

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _vendorSecurityCertRepository.AddVendorSecurityCertAsync(vendorSecurityCert);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateVendorSecurityCertAsync(int id, VendorSecurityCert vendorSecurityCert)
        {
            if(vendorSecurityCert == null)
            {
                throw new ArgumentNullException("VendorSecurityCert cannot be null");
            }

            VendorSecurityCert dbVendorSecurityCert = await GetVendorSecurityCertByIdAsync(id);

            if(!String.IsNullOrEmpty(vendorSecurityCert.CertName))
            {
                dbVendorSecurityCert.CertName = vendorSecurityCert.CertName;
            }

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _vendorSecurityCertRepository.UpdateVendorSecurityCertAsync(dbVendorSecurityCert);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteVendorSecurityCertAsync(int id)
        {
            VendorSecurityCert dbVendorSecurityCert = await GetVendorSecurityCertByIdAsync(id);

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _vendorSecurityCertRepository.DeleteVendorSecurityCertAsync(dbVendorSecurityCert);
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
