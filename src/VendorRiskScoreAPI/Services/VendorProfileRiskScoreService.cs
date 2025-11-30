using Microsoft.EntityFrameworkCore.Storage;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Dtos;
using VendorRiskScoreAPI.Repositories;

namespace VendorRiskScoreAPI.Services
{
    public class VendorProfileRiskScoreService : IVendorProfileRiskScoreService
    {
        private readonly IVendorProfileRiskScoreRepository _vendorProfileRiskScoreRepository;
        private readonly VendorRiskScoreDbContext _context;

        public VendorProfileRiskScoreService(IVendorProfileRiskScoreRepository vendorProfileRiskScoreRepository, 
            VendorRiskScoreDbContext context)
        {
            _vendorProfileRiskScoreRepository = vendorProfileRiskScoreRepository;
            _context = context;
        }

        public async Task<List<VendorProfileRiskScore>> GetVendorProfilesRiskScoresAsync()
        {
            List<VendorProfileRiskScore> vendorProfileRiskScores = await _vendorProfileRiskScoreRepository.GetVendorProfilesRiskScoresAsync();
            return vendorProfileRiskScores;
        }

        public async Task<VendorProfileRiskScore> GetVendorProfileRiskScoreByIdAsync(int id)
        {
            VendorProfileRiskScore vendorProfileRiskScore = await _vendorProfileRiskScoreRepository.GetVendorProfileRiskScoreByIdAsync(id);

            if (vendorProfileRiskScore == null) 
            {
                throw new KeyNotFoundException($"There isn't vendorProfileRiskScore belong with this id:{id}");
            }

            return vendorProfileRiskScore;
        }

        public async Task<VendorProfileRiskScore> AddVendorProfileRiskScoreAsync(VendorProfileRiskScore vendorProfileRiskScore)
        {
            if(vendorProfileRiskScore == null)
            {
                throw new ArgumentNullException("VendorProfileRiskScore cannot be null");
            }

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _vendorProfileRiskScoreRepository.AddVendorProfileRiskScoreAsync(vendorProfileRiskScore);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return vendorProfileRiskScore;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateVendorProfileRiskScoreAsync(int id, VendorProfileRiskScore vendorProfileRiskScore)
        {
            if (vendorProfileRiskScore == null)
            {
                throw new ArgumentNullException("VendorProfileRiskScore cannot be null");
            }

            VendorProfileRiskScore dbVendorProfileRiskScore = await GetVendorProfileRiskScoreByIdAsync(id);

            if(dbVendorProfileRiskScore.VendorName !=  vendorProfileRiskScore.VendorName)
            {
                dbVendorProfileRiskScore.VendorName = vendorProfileRiskScore.VendorName;
            }

            if(dbVendorProfileRiskScore.Financial != vendorProfileRiskScore.Financial)
            {
                dbVendorProfileRiskScore.Financial = vendorProfileRiskScore.Financial;
            }

            if (dbVendorProfileRiskScore.Operational != vendorProfileRiskScore.Operational)
            {
                dbVendorProfileRiskScore.Operational = vendorProfileRiskScore.Operational;
            }

            if (dbVendorProfileRiskScore.Security != vendorProfileRiskScore.Security)
            {
                dbVendorProfileRiskScore.Security = vendorProfileRiskScore.Security;
            }

            if (dbVendorProfileRiskScore.FinalScore != vendorProfileRiskScore.FinalScore)
            {
                dbVendorProfileRiskScore.FinalScore = vendorProfileRiskScore.FinalScore;
            }

            if(dbVendorProfileRiskScore.VendorId != vendorProfileRiskScore.VendorId)
            {
                dbVendorProfileRiskScore.VendorId = vendorProfileRiskScore.VendorId;
            }

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _vendorProfileRiskScoreRepository.UpdateVendorProfileRiskScoreAsync(dbVendorProfileRiskScore);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteVendorProfileRiskScore(int id)
        {
            VendorProfileRiskScore dbVendorProfileRiskScore = await GetVendorProfileRiskScoreByIdAsync(id);

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _vendorProfileRiskScoreRepository.DeleteVendorProfileRiskScore(dbVendorProfileRiskScore);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public Task<VendorProfileRiskScoreResponseDto> CreateVendorProfileRiskScoreResponseDto(VendorProfile vendorProfile)
        {
            VendorProfileRiskScoreResponseDto vendorProfileRiskScoreResponseDto = new VendorProfileRiskScoreResponseDto()
            {
                Vendor = vendorProfile.Name,
                Financial = vendorProfile.VendorProfileRiskScore.Financial,
                Operational = vendorProfile.VendorProfileRiskScore.Operational,
                Security = vendorProfile.VendorProfileRiskScore.Security,
                FinalScore = vendorProfile.VendorProfileRiskScore.FinalScore
            };

            return Task.FromResult(vendorProfileRiskScoreResponseDto);
        }
    }
}
