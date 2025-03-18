using KitBackend.Endpoints;
using KitBackend.Models.Data;
using KitBackend.Models.Requests;
using KitBackend.Models.Responses;
using KitBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KitBackend.Tests.Endpoints
{
    public class AnalysisEndpointsTests
    {
        private readonly Mock<IAnalysisService> _mockAnalysisService;
        private readonly Mock<IFileService> _mockFileService;

        public AnalysisEndpointsTests()
        {
            _mockAnalysisService = new Mock<IAnalysisService>();
            _mockFileService = new Mock<IFileService>();
        }

        [Fact]
        public async Task AnalyzeCode_WithValidContent_ReturnsOkWithReport()
        {
            // Arrange
            var request = new AnalysisRequest
            {
                SourceType = "content",
                Content = "public void Test() { }"
            };

            var expectedReport = new AnalysisReport
            {
                ReportId = Guid.NewGuid(),
                ReadabilityScore = 95,
                ComplexityScore = 90,
                SecurityScore = 100,
                PerformanceScore = 85
            };

            _mockAnalysisService
                .Setup(s => s.GenerateReportAsync(request.Content))
                .ReturnsAsync(expectedReport);

            // Act
            var result = await AnalysisEndpoints.AnalyzeCode(request, _mockAnalysisService.Object, _mockFileService.Object);

            // Assert
            var okResult = Assert.IsType<Ok<AnalysisReport>>(result);
            var returnedReport = Assert.IsType<AnalysisReport>(okResult.Value);
            Assert.Equal(expectedReport.ReportId, returnedReport.ReportId);
            Assert.Equal(expectedReport.ReadabilityScore, returnedReport.ReadabilityScore);
        }

        [Fact]
        public async Task AnalyzeCode_WithValidFileId_ReturnsOkWithReport()
        {
            // Arrange
            var fileId = 1;
            string fileContent = "public void Test() { }";
            byte[] fileContentBytes = Encoding.UTF8.GetBytes(fileContent);

            var request = new AnalysisRequest
            {
                SourceType = "file",
                FileId = fileId
            };

            // Använd den korrekta UploadedFile-modellen
            var file = new UploadedFile
            {
                Id = fileId,
                FileName = "test.cs",
                UploadDate = DateTime.UtcNow,
                FileSize = fileContentBytes.Length,
                Uploader = "test-user",
                FileContent = fileContentBytes,
                ProjectId = Guid.NewGuid(),
                Project = new Project()
            };

            var expectedReport = new AnalysisReport
            {
                ReportId = Guid.NewGuid(),
                ReadabilityScore = 95,
                ComplexityScore = 90,
                SecurityScore = 100,
                PerformanceScore = 85
            };

            _mockFileService
                .Setup(s => s.GetFileById(fileId))
                .ReturnsAsync(file);

            _mockAnalysisService
                .Setup(s => s.GenerateReportAsync(fileContent))
                .ReturnsAsync(expectedReport);

            // Act
            var result = await AnalysisEndpoints.AnalyzeCode(request, _mockAnalysisService.Object, _mockFileService.Object);

            // Assert
            var okResult = Assert.IsType<Ok<AnalysisReport>>(result);
            var returnedReport = Assert.IsType<AnalysisReport>(okResult.Value);
            Assert.Equal(expectedReport.ReportId, returnedReport.ReportId);
        }

        [Fact]
        public async Task AnalyzeCode_WithFileNotFound_ReturnsNotFound()
        {
            // Arrange
            var fileId = 999;

            var request = new AnalysisRequest
            {
                SourceType = "file",
                FileId = fileId
            };

            _mockFileService
                .Setup(s => s.GetFileById(fileId))
                .ReturnsAsync((UploadedFile)null);

            // Act
            var result = await AnalysisEndpoints.AnalyzeCode(request, _mockAnalysisService.Object, _mockFileService.Object);

            // Assert
            Assert.IsType<NotFound<string>>(result);
        }

        [Fact]
        public async Task AnalyzeCode_WithNoContent_ReturnsBadRequest()
        {
            // Arrange
            var request = new AnalysisRequest
            {
                SourceType = "content",
                Content = ""
            };

            // Act
            var result = await AnalysisEndpoints.AnalyzeCode(request, _mockAnalysisService.Object, _mockFileService.Object);

            // Assert
            Assert.IsType<BadRequest<string>>(result);
        }

        [Fact]
        public async Task AnalyzeCode_WhenServiceThrowsException_ReturnsProblem()
        {
            // Arrange
            var request = new AnalysisRequest
            {
                SourceType = "content",
                Content = "public void Test() { }"
            };

            _mockAnalysisService
                .Setup(s => s.GenerateReportAsync(request.Content))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await AnalysisEndpoints.AnalyzeCode(request, _mockAnalysisService.Object, _mockFileService.Object);

            // Assert
            var problemResult = Assert.IsType<ProblemHttpResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
        }

        [Fact]
        public async Task GetAnalysisReport_WithValidId_ReturnsOkWithReport()
        {
            // Arrange
            Guid reportId = Guid.NewGuid();
            var expectedReport = new AnalysisReport
            {
                ReportId = reportId,
                ReadabilityScore = 95,
                ComplexityScore = 90,
                SecurityScore = 100,
                PerformanceScore = 85
            };

            _mockAnalysisService
                .Setup(s => s.GetReportByIdAsync(reportId))
                .ReturnsAsync(expectedReport);

            // Act
            var result = await AnalysisEndpoints.GetAnalysisReport(reportId, _mockAnalysisService.Object);

            // Assert
            var okResult = Assert.IsType<Ok<AnalysisReport>>(result);
            var returnedReport = Assert.IsType<AnalysisReport>(okResult.Value);
            Assert.Equal(expectedReport.ReportId, returnedReport.ReportId);
        }

        [Fact]
        public async Task GetAnalysisReport_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            Guid reportId = Guid.NewGuid();

            _mockAnalysisService
                .Setup(s => s.GetReportByIdAsync(reportId))
                .ThrowsAsync(new KeyNotFoundException($"Analysis report with ID {reportId} not found"));

            // Act
            var result = await AnalysisEndpoints.GetAnalysisReport(reportId, _mockAnalysisService.Object);

            // Assert
            Assert.IsType<ProblemHttpResult>(result);
        }

        [Fact]
        public async Task DownloadReport_WithValidId_ReturnsFileResult()
        {
            // Arrange
            Guid reportId = Guid.NewGuid();
            string tempFilePath = Path.GetTempFileName();
            byte[] pdfContent = Encoding.UTF8.GetBytes("Fake PDF content");
            System.IO.File.WriteAllBytes(tempFilePath, pdfContent);

            var report = new AnalysisReport
            {
                ReportId = reportId,
                PdfPath = tempFilePath
            };

            _mockAnalysisService
                .Setup(s => s.GetReportByIdAsync(reportId))
                .ReturnsAsync(report);

            try
            {
                // Act
                var result = await AnalysisEndpoints.DownloadReport(reportId, _mockAnalysisService.Object);

                // Assert
                var fileResult = Assert.IsType<FileContentHttpResult>(result);
                Assert.Equal("application/pdf", fileResult.ContentType);
                Assert.Equal($"{reportId}.pdf", fileResult.FileDownloadName);
            }
            finally
            {
                // Cleanup
                if (System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }
            }
        }

        [Fact]
        public async Task DownloadReport_WithMissingPdfPath_ReturnsNotFound()
        {
            // Arrange
            Guid reportId = Guid.NewGuid();
            var report = new AnalysisReport
            {
                ReportId = reportId,
                PdfPath = null
            };

            _mockAnalysisService
                .Setup(s => s.GetReportByIdAsync(reportId))
                .ReturnsAsync(report);

            // Act
            var result = await AnalysisEndpoints.DownloadReport(reportId, _mockAnalysisService.Object);

            // Assert
            Assert.IsType<NotFound<string>>(result);
        }

        [Fact]
        public async Task DownloadReport_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            Guid reportId = Guid.NewGuid();

            _mockAnalysisService
                .Setup(s => s.GetReportByIdAsync(reportId))
                .ThrowsAsync(new KeyNotFoundException($"Analysis report with ID {reportId} not found"));

            // Act
            var result = await AnalysisEndpoints.DownloadReport(reportId, _mockAnalysisService.Object);

            // Assert
            Assert.IsType<ProblemHttpResult>(result);
        }
    }
}