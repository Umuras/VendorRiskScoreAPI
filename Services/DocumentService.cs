using Microsoft.EntityFrameworkCore.Storage;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Repositories;

namespace VendorRiskScoreAPI.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly VendorRiskScoreDbContext _context;

        public DocumentService(IDocumentRepository documentRepository, VendorRiskScoreDbContext context)
        {
            _documentRepository = documentRepository;
            _context = context;
        }

        public async Task<List<Document>> GetAllDocumentsAsync()
        {
            List<Document> documents = await _documentRepository.GetAllDocumentsAsync();
            return documents;
        }

        public async Task<Document> GetDocumentByIdAsync(int id)
        {
            Document document = await _documentRepository.GetDocumentByIdAsync(id);

            if(document == null)
            {
                throw new KeyNotFoundException($"There isn't document belong with this id:{id}");
            }

            return document;
        }

        public async Task AddDocumentAsync(Document document)
        {
            if(document == null)
            {
                throw new ArgumentNullException("Document cannot be null");
            }

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _documentRepository.AddDocumentAsync(document);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateDocumentAsync(int id, Document document)
        {
            if(document == null)
            {
                throw new ArgumentNullException("Document cannot be null");
            }

            Document dbDocument = await GetDocumentByIdAsync(id);

            if(document.ContractValid != dbDocument.ContractValid)
            {
                dbDocument.ContractValid = document.ContractValid;
            }

            if(document.PrivacyPolicyValid != dbDocument.PrivacyPolicyValid)
            {
                dbDocument.PrivacyPolicyValid = document.PrivacyPolicyValid;
            }

            if(document.PentestReportValid != dbDocument.PentestReportValid)
            {
                dbDocument.PentestReportValid = document.PentestReportValid;
            }

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _documentRepository.UpdateDocumentAsync(dbDocument);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteDocumentAsync(int id)
        {
            Document dbDocument = await GetDocumentByIdAsync(id);

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _documentRepository.DeleteDocumentAsync(dbDocument);
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
