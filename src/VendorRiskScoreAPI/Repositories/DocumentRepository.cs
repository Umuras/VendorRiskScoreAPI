using Microsoft.EntityFrameworkCore;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;

namespace VendorRiskScoreAPI.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly VendorRiskScoreDbContext _context;

        public DocumentRepository(VendorRiskScoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<Document>> GetAllDocumentsAsync()
        {
            return await _context.Documents.ToListAsync();
        }

        public async Task<Document> GetDocumentByIdAsync(int id)
        {
            Document? document = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);
            return document;
        }
        public async Task AddDocumentAsync(Document document)
        {
            await _context.Documents.AddAsync(document);
        }

        public Task UpdateDocumentAsync(Document document)
        {
            _context.Documents.Update(document);
            return Task.CompletedTask;
        }

        public Task DeleteDocumentAsync(Document document)
        {
            _context.Documents.Remove(document);
            return Task.CompletedTask;
        }
    }
}
