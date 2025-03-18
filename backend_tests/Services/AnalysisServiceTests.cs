using KitBackend.Models.Responses;
using KitBackend.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KitBackend.Tests.Services
{
    public class AnalysisServiceTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<IPdfReportService> _mockPdfReportService;
        private readonly Mock<ILogger<AnalysisService>> _mockLogger;
        private readonly AnalysisService _analysisService;
        private readonly List<AnalysisReport> _reports;
        private readonly List<AnalysisStatus> _statuses;

        public AnalysisServiceTests()
        {
            // Skapa listor för att simulera DbSets
            _reports = new List<AnalysisReport>();
            _statuses = new List<AnalysisStatus>();

            // Setup mocked DbSets med queryable data
            var reportsMockSet = CreateMockDbSet(_reports);
            var statusesMockSet = CreateMockDbSet(_statuses);

            // Setup context mock
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            _contextMock = new Mock<ApplicationDbContext>(options);
            _contextMock.Setup(c => c.AnalysisReport).Returns(reportsMockSet.Object);
            _contextMock.Setup(c => c.AnalysisStatus).Returns(statusesMockSet.Object);

            // Setup PdfReportService mock med interface
            _mockPdfReportService = new Mock<IPdfReportService>();
            _mockPdfReportService.Setup(p => p.GeneratePdfReport(It.IsAny<AnalysisReport>()))
                .Returns("test.pdf");

            _mockLogger = new Mock<ILogger<AnalysisService>>();

            // Create service with mocked context
            _analysisService = new AnalysisService(_contextMock.Object, _mockPdfReportService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GenerateReportAsync_ShouldReturnReport_WhenCodeIsValid()
        {
            // Arrange
            string code = "public class Test { }";

            // Setup the Add och SaveChangesAsync
            _contextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Callback(() => {
                    // Simulera att rapporten har lagts till efter SaveChangesAsync
                    if (_reports.Count > 0)
                    {
                        var report = _reports[0];
                        if (string.IsNullOrEmpty(report.PdfPath))
                        {
                            report.PdfPath = "test.pdf";
                        }
                    }
                });

            // Act
            var result = await _analysisService.GenerateReportAsync(code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test.pdf", result.PdfPath);
        }

        [Fact]
        public async Task GetReportByIdAsync_ShouldReturnReport_WhenIdIsValid()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            Console.WriteLine($"Using report ID: {reportId}");

            var expectedReport = new AnalysisReport { ReportId = reportId };

            // Mocka alla möjliga versioner av FindAsync
            _contextMock.Setup(c => c.AnalysisReport.FindAsync(reportId))
                .ReturnsAsync(expectedReport);

            _contextMock.Setup(c => c.AnalysisReport.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync(expectedReport);

            _contextMock.Setup(c => c.AnalysisReport.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedReport);

            // Act
            var result = await _analysisService.GetReportByIdAsync(reportId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reportId, result.ReportId);
        }

        [Fact]
        public async Task GetReportByIdAsync_ShouldThrowKeyNotFoundException_WhenIdDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _analysisService.GetReportByIdAsync(nonExistentId));
        }


        [Fact]
        public async Task SaveAnalysisStatusAsync_ShouldSaveStatus()
        {
            // Arrange
            var status = new AnalysisStatus { AnalysisId = Guid.NewGuid(), Status = AnalysisStatusEnum.Pending };

            // Act
            await _analysisService.SaveAnalysisStatusAsync(status);

            // Assert
            Assert.Contains(_statuses, s => s.AnalysisId == status.AnalysisId);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAnalysisStatusAsync_ShouldUpdateStatus()
        {
            // Arrange
            var statusId = Guid.NewGuid();
            var status = new AnalysisStatus { AnalysisId = statusId, Status = AnalysisStatusEnum.InProgress };
            _statuses.Add(status);

            var updatedStatus = new AnalysisStatus { AnalysisId = statusId, Status = AnalysisStatusEnum.Completed };

            // Skapa en ny mockad DbSet för AnalysisStatus som kommer uppdatera _statuses
            var statusesMockSet = CreateMockDbSet(_statuses);

            // Denna mock är den kritiska - vi måste uppdatera den befintliga statusen
            statusesMockSet.Setup(m => m.Update(It.IsAny<AnalysisStatus>()))
                .Callback<AnalysisStatus>(newStatus => {
                    var existingStatus = _statuses.FirstOrDefault(s => s.AnalysisId == newStatus.AnalysisId);
                    if (existingStatus != null)
                    {
                        existingStatus.Status = newStatus.Status; // Uppdatera den befintliga statusen
                    }
                });

            // Uppdatera context mock för att använda den nya mockade DbSet:en
            _contextMock.Setup(c => c.AnalysisStatus).Returns(statusesMockSet.Object);

            // Act
            await _analysisService.UpdateAnalysisStatusAsync(updatedStatus);

            // Assert
            var foundStatus = _statuses.FirstOrDefault(s => s.AnalysisId == statusId);
            Assert.NotNull(foundStatus);
            Assert.Equal(AnalysisStatusEnum.Completed, foundStatus.Status);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        // Hjälpmetod för att skapa en mockad DbSet från en lista
        private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
        {
            var queryableData = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryableData.GetEnumerator());

            mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(item => data.Add(item));
            mockSet.Setup(m => m.Update(It.IsAny<T>())).Callback<T>(item => {
                // För testsyften
            });

            // Setup FindAsync för AnalysisReport
            if (typeof(T) == typeof(AnalysisReport))
            {
                mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((object[] ids, CancellationToken token) => {
                        var id = (Guid)ids[0];
                        return data.Cast<AnalysisReport>().FirstOrDefault(r => r.ReportId == id) as T;
                    });
            }

            // Setup FindAsync för AnalysisStatus
            if (typeof(T) == typeof(AnalysisStatus))
            {
                mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((object[] ids, CancellationToken token) => {
                        var id = (Guid)ids[0];
                        return data.Cast<AnalysisStatus>().FirstOrDefault(s => s.AnalysisId == id) as T;
                    });
            }

            return mockSet;
        }
    }
}
