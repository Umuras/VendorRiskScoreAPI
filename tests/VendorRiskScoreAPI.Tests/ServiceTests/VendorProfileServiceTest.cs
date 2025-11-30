using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Domain.ValueObjects;
using VendorRiskScoreAPI.Dtos;
using VendorRiskScoreAPI.Enums;
using VendorRiskScoreAPI.Repositories;
using VendorRiskScoreAPI.Services;
using Document = VendorRiskScoreAPI.Domain.Entities.Document;

namespace VendorRiskScoreAPI.Tests.ServiceTests
{
    public class VendorProfileServiceTest
    {
        Mock<IVendorProfileRepository> mockRepo;
        Mock<VendorRiskScoreDbContext> mockDb;
        Mock<IRuleEngineService> mockRuleEngine;
        Mock<IRiskAssessmentService> mockRiskAssessment;
        Mock<IDocumentService> mockDocumentService;
        Mock<IVendorProfileRiskScoreService> mockVendorProfileRiskScoreService;
        VendorProfileService service;

        public VendorProfileServiceTest()
        {
            mockRepo = new Mock<IVendorProfileRepository>();
            mockDb = new Mock<VendorRiskScoreDbContext>();
            mockRuleEngine = new Mock<IRuleEngineService>();
            mockRiskAssessment = new Mock<IRiskAssessmentService>();
            mockDocumentService = new Mock<IDocumentService>();
            mockVendorProfileRiskScoreService = new Mock<IVendorProfileRiskScoreService>();
            service = new VendorProfileService(mockRepo.Object, mockDb.Object,
                mockRuleEngine.Object, mockRiskAssessment.Object, mockDocumentService.Object, mockVendorProfileRiskScoreService.Object);
        }

        [Fact(DisplayName = "GetVendorProfilesAsync metodu tüm vendor profilleri döndürmeli")]
        public async Task GetAllVendorProfileServiceTest()
        {
            //Arrange
            List<VendorProfile> dummyDatas = new List<VendorProfile>
            {
                new VendorProfile { Id = 1, Name = "Test Vendor 1", FinancialHealth = 50, MajorIncidents = 1, SlaUpTime = 90},
                new VendorProfile { Id = 2, Name = "Test Vendor 2", FinancialHealth = 20, MajorIncidents = 9, SlaUpTime = 95},
                new VendorProfile { Id = 3, Name = "Test Vendor 3", FinancialHealth = 35, MajorIncidents = 10, SlaUpTime = 70},
                new VendorProfile { Id = 4, Name = "Test Vendor 4", FinancialHealth = 45, MajorIncidents = 0, SlaUpTime = 30}
            };

            mockRepo.Setup(r => r.GetVendorProfilesAsync()).ReturnsAsync(dummyDatas);

            //Act
            List<VendorProfile> result = await service.GetVendorProfilesAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.Equal("Test Vendor 4", result[3].Name);
            Assert.Equal(50, result[0].FinancialHealth);

            mockRepo.Verify(r => r.GetVendorProfilesAsync(), Times.Once);
        }

        [Fact(DisplayName = "GetVendorProfileByIdAsync metodu verilen iddeki vendor profilini döndürmeli")]
        public async Task GetVendorProfileByIdServiceTest()
        {
            //Arrange
            Document document = new Document()
            {
                ContractValid = true,
                PrivacyPolicyValid = true,
                PentestReportValid = true,
            };
            VendorProfile vendorProfile = new VendorProfile
            {
                Id = 1,
                Name = "Test Vendor 1",
                FinancialHealth = 50,
                MajorIncidents = 1,
                SlaUpTime = 90,
                Document = document
            };

            mockRepo.Setup(r => r.GetVendorProfileByIdAsync(1)).ReturnsAsync(vendorProfile);

            //Act
            VendorProfile result = await service.GetVendorProfileByIdAsync(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(vendorProfile.Id, result.Id);
            Assert.Equal(vendorProfile.FinancialHealth, result.FinancialHealth);
            Assert.True(result.Document.ContractValid);

            mockRepo.Verify(r => r.GetVendorProfileByIdAsync(1), Times.Once);
        }

        [Fact(DisplayName = "Geçersiz id verildiğinde GetVendorProfileByIdAsync hata fırlatmalı")]
        public async Task GetVendorProfileByIdService_NotFound_ShouldThrow()
        {
            //Arrange
            int id = 1;
            mockRepo.Setup(r => r.GetVendorProfileByIdAsync(id)).ReturnsAsync((VendorProfile)null);

            //Act & Assert
            var exception = Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetVendorProfileByIdAsync(id));

            Assert.Equal($"There isn't vendorProfile belong with this id:{id}", exception.Result.Message);

            mockRepo.Verify(r => r.GetVendorProfileByIdAsync(id), Times.Once);
        }

        [Fact(DisplayName = "Vendor Profile database'e ekleniyor mu?")]
        public async Task AddVendorProfileServiceTest()
        {
            // Arrange (test için sahte veri ve mock davranışları hazırlanıyor)
            VendorProfileRequestDto vendorProfileRequest = new VendorProfileRequestDto
            {
                Name = "Test Vendor",
                FinancialHealth = 50,
                MajorIncidents = 1,
                SlaUptime = 90,
                Document = new DocumentDto { ContractValid = true, PrivacyPolicyValid = true, PentestReportValid = true },
                SecurityCerts = new List<string> { "ISO27001" }
            };

            Document document = new Document();
            document.ContractValid = vendorProfileRequest.Document.ContractValid;
            document.PrivacyPolicyValid = vendorProfileRequest.Document.PrivacyPolicyValid;
            document.PentestReportValid = vendorProfileRequest.Document.PentestReportValid;

            List<VendorSecurityCert> vendorSecurityCerts = new List<VendorSecurityCert>();
            VendorSecurityCert vendorSecurityCert = new VendorSecurityCert();
            vendorSecurityCert.CertName = vendorProfileRequest.SecurityCerts.First();
            vendorSecurityCerts.Add(vendorSecurityCert);

            decimal finalScore = 0.5m;
            string riskLevel = "Medium";

            RiskAssessment riskAssessment = new RiskAssessment();
            riskAssessment.RiskScore = finalScore;

            //Service içinde bu kullanılan objeler kendi içinde oluşturulduğu için Moq'da referans bazlı karşılaştırma yaptığı için
            //aynı referanslara sahip olmadıkları için It.IsAny<ObjeSınıfı>() şeklinde kullanıyoruz. Hem Setupta hem Verifyda.
            mockRiskAssessment.Setup(r => r.CalculateFinalScore(
                               vendorProfileRequest.FinancialHealth,
                               vendorProfileRequest.SlaUptime,
                               vendorProfileRequest.MajorIncidents,
                               It.IsAny<List<VendorSecurityCert>>(),
                               It.IsAny<Document>())).Returns(finalScore);

            mockRiskAssessment.Setup(r => r.CalculateRiskLevel(It.IsAny<decimal>())).Returns(riskLevel);

            RiskScores riskScores = new RiskScores();
            riskScores.Security = 0.4m;
            riskScores.Financial = 0.6m;
            riskScores.Operational = 0.2m;
            riskScores.FinalScore = 0.5m;

            mockRiskAssessment.Setup(r => r.CalculateVendorProfileRiskScores(
                                    It.IsAny<string>(),
                                    It.IsAny<int>(),
                                    It.IsAny<int>(),
                                    It.IsAny<int>(),
                                    It.IsAny<List<VendorSecurityCert>>(),
                                    It.IsAny<Document>())).ReturnsAsync(riskScores);

            VendorProfileRiskScore vendorProfileRiskScore = new VendorProfileRiskScore()
            {
                VendorName = vendorProfileRequest.Name,
                Financial = riskScores.Financial,
                Operational = riskScores.Operational,
                Security = riskScores.Security,
                FinalScore = riskScores.FinalScore
            };

            VendorProfile vendorProfile = new VendorProfile()
            {
                Name = vendorProfileRequest.Name,
                FinancialHealth = vendorProfileRequest.FinancialHealth,
                MajorIncidents = vendorProfileRequest.MajorIncidents,
                SlaUpTime = vendorProfileRequest.SlaUptime,
                Document = document,
                SecurityCerts = vendorSecurityCerts,
                RiskAssessment = riskAssessment,
                VendorProfileRiskScore = vendorProfileRiskScore
            };


            mockRepo.Setup(r => r.AddVendorProfileAsync(It.IsAny<VendorProfile>())).ReturnsAsync(vendorProfile);


            // Transaction mock: Commit ve Rollback çağrıldığında herhangi bir işlem yapmadan başarılı dönecek
            var mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(default)).Returns(Task.CompletedTask);
            mockTransaction.Setup(t => t.RollbackAsync(default)).Returns(Task.CompletedTask);

            // DatabaseFacade mock: BeginTransactionAsync çağrıldığında mockTransaction döndürülecek
            var mockDatabaseFacade = new Mock<DatabaseFacade>(mockDb.Object);
            mockDatabaseFacade
                .Setup(db => db.BeginTransactionAsync(default))
                .ReturnsAsync(mockTransaction.Object);

            // DbContext'in Database özelliği çağrıldığında mock database facade dönecek
            mockDb.Setup(db => db.Database).Returns(mockDatabaseFacade.Object);


            //Act
            VendorProfile result = await service.AddVendorProfileAsync(vendorProfileRequest);

            // Assert (beklenen davranışlar doğrulanıyor)
            Assert.NotNull(result);
            Assert.Equal(vendorProfile.Name, result.Name);
            Assert.Equal(vendorProfile.Id, result.Id);
            Assert.Equal(vendorProfile.MajorIncidents, result.MajorIncidents);
            Assert.Equal(vendorProfile.FinancialHealth, result.FinancialHealth);
            Assert.Equal(vendorProfile.SecurityCerts.First().CertName, result.SecurityCerts.First().CertName);
            Assert.Equal(vendorProfile.Document.ContractValid, result.Document.ContractValid);

            mockRiskAssessment.Verify(r => r.CalculateVendorProfileRiskScores(
                               It.IsAny<string>(),
                               It.IsAny<int>(),
                               It.IsAny<int>(),
                               It.IsAny<int>(),
                               It.IsAny<List<VendorSecurityCert>>(),
                               It.IsAny<Document>()
                               ));


            mockRiskAssessment.Verify(r => r.CalculateFinalScore(
                               vendorProfileRequest.FinancialHealth,
                               vendorProfileRequest.SlaUptime,
                               vendorProfileRequest.MajorIncidents,
                               It.IsAny<List<VendorSecurityCert>>(),
                               It.IsAny<Document>()), Times.Once);

            mockRiskAssessment.Verify(r => r.CalculateRiskLevel(It.IsAny<decimal>()), Times.Once);
            mockRepo.Verify(r => r.AddVendorProfileAsync(It.IsAny<VendorProfile>()), Times.Once);
            mockTransaction.Verify(t => t.CommitAsync(default), Times.Once);
        }

        [Fact(DisplayName = "Vendor Profile Güncellenebiliyor mu?")]
        public async Task UpdateVendorProfileServiceTest()
        {
            //Arrange

            VendorProfileRequestDto vendorProfileRequest = new VendorProfileRequestDto
            {
                Name = "Test Vendor",
                FinancialHealth = 50,
                MajorIncidents = 1,
                SlaUptime = 90,
                Document = new DocumentDto { ContractValid = true, PrivacyPolicyValid = true, PentestReportValid = true },
                SecurityCerts = new List<string> { "ISO27001" }
            };

            List<VendorSecurityCert> vendorSecurityCerts = new List<VendorSecurityCert>();
            vendorSecurityCerts.Add(new VendorSecurityCert() { CertName = vendorProfileRequest.SecurityCerts.First() });

            VendorProfile dbVendorProfile = new VendorProfile()
            {
                Id = 1,
                Name = "Test",
                FinancialHealth = 60,
                MajorIncidents = 2,
                SlaUpTime = 60,
                Document = new Document { ContractValid = false, PrivacyPolicyValid = false, PentestReportValid = false },
                SecurityCerts = vendorSecurityCerts
            };

            Document dbDocument = new Document()
            {
                Id = 1,
                ContractValid = true,
                PrivacyPolicyValid = false,
                PentestReportValid = true
            };

            mockDocumentService.Setup(d => d.GetDocumentByIdAsync(It.IsAny<int>())).ReturnsAsync(dbDocument);
            mockRepo.Setup(r => r.GetVendorProfileByIdAsync(1)).ReturnsAsync(dbVendorProfile);

            dbDocument.ContractValid = vendorProfileRequest.Document.ContractValid;
            dbDocument.PrivacyPolicyValid = vendorProfileRequest.Document.PrivacyPolicyValid;
            dbDocument.PentestReportValid = vendorProfileRequest.Document.PentestReportValid;

            decimal finalScore = 0.5m;
            string riskLevel = "Medium";

            RiskAssessment dbRiskAssessment = new RiskAssessment();
            dbRiskAssessment.Id = 1;
            dbRiskAssessment.RiskScore = finalScore;
            dbRiskAssessment.RiskLevel = riskLevel;

            mockRiskAssessment.Setup(r => r.GetRiskAssessmentByIdAsync(1)).ReturnsAsync(dbRiskAssessment);

            mockRiskAssessment.Setup(r => r.CalculateFinalScore(
                               vendorProfileRequest.FinancialHealth,
                               vendorProfileRequest.SlaUptime,
                               vendorProfileRequest.MajorIncidents,
                               It.IsAny<List<VendorSecurityCert>>(),
                               It.IsAny<Document>())).Returns(finalScore);

            mockRiskAssessment.Setup(r => r.CalculateRiskLevel(It.IsAny<decimal>())).Returns(riskLevel);

            dbVendorProfile.RiskAssessment = dbRiskAssessment;

            VendorProfileRiskScore dbVendorProfileRiskScore = new VendorProfileRiskScore()
            {
                Id = 1,
                VendorName = "Test Vendor",
                Financial = 0.5m,
                Operational = 0.8m,
                Security = 0.4m,
                FinalScore = 0.6m,
            };

            mockVendorProfileRiskScoreService
            .Setup(r => r.GetVendorProfileRiskScoreByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(dbVendorProfileRiskScore);

            RiskScores riskScores = new RiskScores();
            riskScores.Security = 0.4m;
            riskScores.Financial = 0.6m;
            riskScores.Operational = 0.2m;
            riskScores.FinalScore = 0.5m;

            mockRiskAssessment.Setup(r => r.CalculateVendorProfileRiskScores(
                                    It.IsAny<string>(),
                                    It.IsAny<int>(),
                                    It.IsAny<int>(),
                                    It.IsAny<int>(),
                                    It.IsAny<List<VendorSecurityCert>>(),
                                    It.IsAny<Document>())).ReturnsAsync(riskScores);

            dbVendorProfileRiskScore.Financial = riskScores.Financial;
            dbVendorProfileRiskScore.Operational = riskScores.Operational;
            dbVendorProfileRiskScore.Security = riskScores.Security;
            dbVendorProfileRiskScore.FinalScore = riskScores.FinalScore;

            dbVendorProfile.VendorProfileRiskScore = dbVendorProfileRiskScore;

            mockRepo.Setup(r => r.UpdateVendorProfileAsync(dbVendorProfile)).Returns(Task.CompletedTask);

            // Transaction mock: Commit ve Rollback çağrıldığında herhangi bir işlem yapmadan başarılı dönecek
            var mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(default)).Returns(Task.CompletedTask);
            mockTransaction.Setup(t => t.RollbackAsync(default)).Returns(Task.CompletedTask);

            // DatabaseFacade mock: BeginTransactionAsync çağrıldığında mockTransaction döndürülecek
            var mockDatabaseFacade = new Mock<DatabaseFacade>(mockDb.Object);
            mockDatabaseFacade
                .Setup(db => db.BeginTransactionAsync(default))
                .ReturnsAsync(mockTransaction.Object);

            // DbContext'in Database özelliği çağrıldığında mock database facade dönecek
            mockDb.Setup(db => db.Database).Returns(mockDatabaseFacade.Object);


            //Act
            await service.UpdateVendorProfileAsync(1, vendorProfileRequest);

            //Assert
            Assert.NotNull(dbVendorProfile);
            Assert.Equal(vendorProfileRequest.Name, dbVendorProfile.Name);
            Assert.Equal(vendorProfileRequest.FinancialHealth, dbVendorProfile.FinancialHealth);
            Assert.Equal(vendorProfileRequest.SlaUptime, dbVendorProfile.SlaUpTime);
            Assert.Equal(vendorProfileRequest.MajorIncidents, dbVendorProfile.MajorIncidents);
            Assert.Equal(vendorProfileRequest.Document.ContractValid, dbVendorProfile.Document.ContractValid);


            mockRiskAssessment.Verify(r => r.CalculateVendorProfileRiskScores(
                               It.IsAny<string>(),
                               It.IsAny<int>(),
                               It.IsAny<int>(),
                               It.IsAny<int>(),
                               It.IsAny<List<VendorSecurityCert>>(),
                               It.IsAny<Document>()
                               ));


            mockRiskAssessment.Verify(r => r.CalculateFinalScore(
                               vendorProfileRequest.FinancialHealth,
                               vendorProfileRequest.SlaUptime,
                               vendorProfileRequest.MajorIncidents,
                               It.IsAny<List<VendorSecurityCert>>(),
                               It.IsAny<Document>()), Times.Once);

            mockRiskAssessment.Verify(r => r.CalculateRiskLevel(It.IsAny<decimal>()), Times.Once);
            mockRiskAssessment.Verify(r => r.GetRiskAssessmentByIdAsync(1), Times.Once);
            mockVendorProfileRiskScoreService.Verify(r => r.GetVendorProfileRiskScoreByIdAsync(1), Times.Once);
            mockDocumentService.Verify(r => r.GetDocumentByIdAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(r => r.GetVendorProfileByIdAsync(1), Times.Once);
            mockRepo.Verify(r => r.UpdateVendorProfileAsync(It.IsAny<VendorProfile>()), Times.Once);
            mockTransaction.Verify(t => t.CommitAsync(default), Times.Once);
        }

        [Fact(DisplayName = "Vendor Profile silinebilmeli")]
        public async Task DeleteVendorProfileServiceTest()
        {
            // Arrange (test için sahte veri ve mock davranışları hazırlanıyor)

            // Test sırasında repo'nun döndürmesi için sahte bir VendorProfile nesnesi oluşturuyoruz
            VendorProfile vendorProfile = new VendorProfile
            {
                Id = 1,
                Name = "Test Vendor 1",
                FinancialHealth = 50,
                MajorIncidents = 1,
                SlaUpTime = 90
            };

            // Repo mock: ID'ye göre vendor profili getirildiğinde bu vendorProfile dönecek
            mockRepo.Setup(r => r.GetVendorProfileByIdAsync(1)).ReturnsAsync(vendorProfile);
            // Repo mock: Silme işlemi çağrıldığında hata vermeden işlem tamamlanmış gibi dönecek
            mockRepo.Setup(r => r.DeleteVendorProfile(vendorProfile)).Returns(Task.CompletedTask);

            // Transaction mock: Commit ve Rollback çağrıldığında herhangi bir işlem yapmadan başarılı dönecek
            var mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(default)).Returns(Task.CompletedTask);
            mockTransaction.Setup(t => t.RollbackAsync(default)).Returns(Task.CompletedTask);

            // DatabaseFacade mock: BeginTransactionAsync çağrıldığında mockTransaction döndürülecek
            var mockDatabaseFacade = new Mock<DatabaseFacade>(mockDb.Object);
            mockDatabaseFacade
                .Setup(db => db.BeginTransactionAsync(default))
                .ReturnsAsync(mockTransaction.Object);

            // DbContext'in Database özelliği çağrıldığında mock database facade dönecek
            mockDb.Setup(db => db.Database).Returns(mockDatabaseFacade.Object);

            // Act (test edilen metod çağrılıyor)
            // Burada service içindeki silme ve transaction işlemleri çalışacak
            await service.DeleteVendorProfile(1);

            // Assert (beklenen davranışlar doğrulanıyor)

            // Vendor profili getiren metodun 1 kez çağrıldığını doğrula
            mockRepo.Verify(r => r.GetVendorProfileByIdAsync(1), Times.Once);
            // Silme metodunun gerçekten çalıştığını doğrula
            mockRepo.Verify(r => r.DeleteVendorProfile(vendorProfile), Times.Once);
            // Transaction'ın commit edildiğini doğrula
            mockTransaction.Verify(t => t.CommitAsync(default), Times.Once);
        }
    }
}
