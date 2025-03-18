// För Alternativ 1
using KitBackend.Models.Data;
using KitBackend.Models.Requests;
using KitBackend.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;

namespace KitBackend.Tests.TestHelpers
{
    public class TestApplicationDbContext : ApplicationDbContext
    {
        public TestApplicationDbContext() : base(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options)
        { }

        // Mock DbSets for testing
        public Mock<DbSet<AnalysisReport>> MockAnalysisReport { get; set; } = new Mock<DbSet<AnalysisReport>>();
        public Mock<DbSet<AnalysisStatus>> MockAnalysisStatus { get; set; } = new Mock<DbSet<AnalysisStatus>>();
        public Mock<DbSet<CodeSnippet>> MockCodeSnippet { get; set; } = new Mock<DbSet<CodeSnippet>>();
        public Mock<DbSet<UploadedFile>> MockUploadedFile { get; set; } = new Mock<DbSet<UploadedFile>>();

        // Override the base properties to return mock objects
        public override DbSet<AnalysisReport> AnalysisReport => MockAnalysisReport.Object;
        public override DbSet<AnalysisStatus> AnalysisStatus => MockAnalysisStatus.Object;
        public override DbSet<CodeSnippet> CodeSnippet => MockCodeSnippet.Object;
        public override DbSet<UploadedFile> UploadedFile => MockUploadedFile.Object;
    }
}
