using KitBackend.Models.Responses;
using KitBackend.Models.Data;
using KitBackend.Services;
using KitBackend.Services.AnalysisHelpers;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KitBackend.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly ApplicationDbContext _context;
        // private readonly MLModelService _mlModelService; // ML-koden utkommenterad
        private readonly PdfReportService _pdfReportService;
        private readonly ILogger<AnalysisService> _logger;

        public AnalysisService(ApplicationDbContext context, /*MLModelService mlModelService,*/ PdfReportService pdfReportService, ILogger<AnalysisService> logger)
        {
            _context = context;
            // _mlModelService = mlModelService; // ML-koden utkommenterad
            _pdfReportService = pdfReportService;
            _logger = logger;
        }

        public async Task<AnalysisReport> GenerateReportAsync(string code)
        {
            var analysisStatus = new AnalysisStatus
            {
                Status = AnalysisStatusEnum.Pending,
                AnalysisId = Guid.NewGuid()
            };

            await SaveAnalysisStatusAsync(analysisStatus);

            try
            {
                analysisStatus.Status = AnalysisStatusEnum.InProgress;
                await UpdateAnalysisStatusAsync(analysisStatus);

                // Generate analysis
                List<string> issues = RoslynAnalyzer.Analyze(code);

                if (code.Contains("import") || code.Contains("export"))
                {
                    var eslintReport = ESLintAnalyzer.Analyze(code);
                    issues.AddRange(ParseESLintOutput(eslintReport));
                }

                var securityIssues = SecurityHelper.PerformSecurityAnalysis(code);
                var performanceIssues = PerformanceHelper.PerformPerformanceAnalysis(code);
                var readabilityIssues = ReadabilityHelper.AnalyzeReadability(code);

                int readabilityScore = ReadabilityHelper.CalculateReadabilityScore(code);
                int securityScore = SecurityHelper.CalculateSecurityScore(code);
                int performanceScore = PerformanceHelper.CalculatePerformanceScore(code);
                int complexityScore = ComplexityHelper.CalculateComplexityScore(code);

                // Använd ML-modellen för att generera en förklaring
                // var explanationDict = _mlModelService.Predict(code); // ML-koden utkommenterad
                // string explanation = string.Join("; ", explanationDict.Select(kvp => $"{kvp.Key}: {kvp.Value}")); // ML-koden utkommenterad
                string explanation = "Explanation based on analysis results."; // Placeholder explanation

                var report = new AnalysisReport
                {
                    ReportId = Guid.NewGuid(),
                    Issues = issues,
                    SecurityIssues = securityIssues,
                    PerformanceIssues = performanceIssues,
                    ReadabilityIssues = readabilityIssues,
                    ReadabilityScore = readabilityScore,
                    SecurityScore = securityScore,
                    PerformanceScore = performanceScore,
                    ComplexityScore = complexityScore,
                    BestPracticesFeedback = "Ensure your code follows best practices and optimal performance guidelines.",
                    Explanation = explanation, // Lägg till förklaringen i rapporten
                    PdfPath = string.Empty // Initialisera PdfPath
                };

                await _context.AnalysisReport.AddAsync(report);
                await _context.SaveChangesAsync();

                // Lägg till den nya koden och dess analysresultat till träningsdata och träna om modellen
                // var newTrainingData = new CodeAnalysisData // ML-koden utkommenterad
                // {
                //     Code = code,
                //     ComplexityScore = complexityScore,
                //     ReadabilityScore = readabilityScore,
                //     SecurityScore = securityScore,
                //     PerformanceScore = performanceScore,
                //     Explanation = explanation
                // };
                // _mlModelService.AddTrainingData(newTrainingData); // ML-koden utkommenterad

                analysisStatus.Status = AnalysisStatusEnum.Completed;
                await UpdateAnalysisStatusAsync(analysisStatus);

                // Generera PDF-rapport
                try
                {
                    var pdfPath = _pdfReportService.GeneratePdfReport(report);
                    report.PdfPath = pdfPath; // Uppdatera PdfPath
                    await _context.SaveChangesAsync(); // Spara ändringen av PdfPath
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to generate PDF report");
                    throw new Exception("Failed to generate PDF report: " + ex.Message);
                }

                // Returnera rapporten och PDF-sökvägen
                return report;
            }
            catch (Exception ex)
            {
                analysisStatus.Status = AnalysisStatusEnum.Failed;
                await UpdateAnalysisStatusAsync(analysisStatus);

                _logger.LogError(ex, "Code analysis failed");
                throw new Exception("Code analysis failed: " + ex.Message);
            }
        }

        public async Task<AnalysisReport> GetReportByIdAsync(Guid id)
        {
            return await _context.AnalysisReport.FindAsync(id);
        }

        public async Task SaveAnalysisStatusAsync(AnalysisStatus status)
        {
            _context.AnalysisStatus.Add(status);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAnalysisStatusAsync(AnalysisStatus status)
        {
            _context.AnalysisStatus.Update(status);
            await _context.SaveChangesAsync();
        }

        private List<string> ParseESLintOutput(string eslintOutput)
        {
            return eslintOutput.Split('\n').Where(line => line.Contains("error")).ToList();
        }
    }
}
