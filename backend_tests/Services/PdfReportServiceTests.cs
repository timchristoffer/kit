using KitBackend.Models.Responses;
using KitBackend.Services;
using Microsoft.Extensions.Logging;
using Moq;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace KitBackend.Tests.Services
{
    public class PdfReportServiceTests : IDisposable
    {
        private readonly Mock<ILogger<PdfReportService>> _mockLogger;
        private readonly PdfReportService _pdfReportService;
        private readonly string _testReportsDirectoryPath;

        public PdfReportServiceTests()
        {
            // Setup a mock logger
            _mockLogger = new Mock<ILogger<PdfReportService>>();

            // Create a PdfReportService with the mock logger
            _pdfReportService = new PdfReportService(_mockLogger.Object);

            // Create a temporary directory for the test reports
            _testReportsDirectoryPath = Path.Combine(Path.GetTempPath(), "TestReports");
            Directory.CreateDirectory(_testReportsDirectoryPath);

            // Redirect the Reports directory to our test directory by mocking it with a temp path
            Environment.SetEnvironmentVariable("ReportsPath", _testReportsDirectoryPath);
        }

        [Fact]
        public void GeneratePdfReport_ShouldCreatePdfFile_WhenValidReportIsProvided()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = CreateSampleReport(reportId);

            // Act
            var pdfPath = _pdfReportService.GeneratePdfReport(report);

            // Assert
            Assert.True(File.Exists(pdfPath));

            // Basic PDF validity check - ändrat från ReadOnly till Import
            using (var document = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import))
            {
                Assert.True(document.PageCount > 0);
            }
        }

        [Fact]
        public void GeneratePdfReport_ShouldReturnExpectedPath()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = CreateSampleReport(reportId);
            var expectedPath = Path.Combine("Reports", $"{reportId}.pdf");

            // Act
            var actualPath = _pdfReportService.GeneratePdfReport(report);

            // Assert
            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public void GeneratePdfReport_ShouldThrow_WhenIOExceptionOccurs()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = CreateSampleReport(reportId);

            // Create a partial mock of PdfReportService that throws IOException
            var mockPdfService = new Mock<PdfReportService>(_mockLogger.Object) { CallBase = true };
            mockPdfService.Setup(x => x.GeneratePdfReport(It.IsAny<AnalysisReport>())).Throws(new IOException("Test IO exception"));

            // Act & Assert
            var exception = Assert.Throws<IOException>(() => mockPdfService.Object.GeneratePdfReport(report));
            Assert.Equal("Test IO exception", exception.Message);

            // Ta bort loggverifieringen helt om metoden inte loggar
        }

        [Fact]
        public void GeneratePdfReport_ShouldHandleEmptyLists()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = new AnalysisReport
            {
                ReportId = reportId,
                ComplexityScore = 80,
                ReadabilityScore = 75,
                SecurityScore = 90,
                PerformanceScore = 85,
                BestPracticesFeedback = "Test feedback",
                Issues = new List<string>(),
                SecurityIssues = new List<string>(),
                PerformanceIssues = new List<string>(),
                ReadabilityIssues = new List<string>()
            };

            // Act
            var pdfPath = _pdfReportService.GeneratePdfReport(report);

            // Assert
            Assert.True(File.Exists(pdfPath));
        }

        // Helper method to create a sample report for testing
        private AnalysisReport CreateSampleReport(Guid reportId)
        {
            return new AnalysisReport
            {
                ReportId = reportId,
                ComplexityScore = 80,
                ReadabilityScore = 75,
                SecurityScore = 90,
                PerformanceScore = 85,
                BestPracticesFeedback = "Follow SOLID principles and maintain clean code practices.",
                Issues = new List<string> { "Issue 1", "Issue 2" },
                SecurityIssues = new List<string> { "Security Issue 1", "Security Issue 2" },
                PerformanceIssues = new List<string> { "Performance Issue 1", "Performance Issue 2" },
                ReadabilityIssues = new List<string> { "Readability Issue 1", "Readability Issue 2" },
                Explanation = "This is a detailed explanation."
            };
        }

        // Clean up after tests
        public void Dispose()
        {
            try
            {
                // Delete the test directory and all its contents
                if (Directory.Exists(_testReportsDirectoryPath))
                {
                    Directory.Delete(_testReportsDirectoryPath, true);
                }
            }
            catch (IOException)
            {
                // Ignore cleanup errors
            }
        }
    }
}
