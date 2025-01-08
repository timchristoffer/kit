using KitBackend.Models.Responses;
using KitBackend.Services;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitBackend.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly ApplicationDbContext _context;

        public AnalysisService(ApplicationDbContext context)
        {
            _context = context;
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

                // Generera analys
                List<string> issues = RoslynAnalyzer.Analyze(code);

                if (code.Contains("import") || code.Contains("export"))
                {
                    var eslintReport = ESLintAnalyzer.Analyze(code);
                    issues.AddRange(ParseESLintOutput(eslintReport));
                }

                var securityIssues = PerformSecurityAnalysis(code);
                var performanceIssues = PerformPerformanceAnalysis(code);
                var readabilityIssues = AnalyzeReadability(code);

                // Beräkna poäng för varje kategori
                int readabilityScore = CalculateReadabilityScore(code);
                int securityScore = CalculateSecurityScore(code);
                int performanceScore = CalculatePerformanceScore(code);

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
                    ComplexityScore = CalculateComplexityScore(code),
                    BestPracticesFeedback = "Ensure your code follows best practices and optimal performance guidelines."
                };

                await _context.AnalysisReport.AddAsync(report);
                await _context.SaveChangesAsync();

                analysisStatus.Status = AnalysisStatusEnum.Completed;
                await UpdateAnalysisStatusAsync(analysisStatus);

                return report;
            }
            catch (Exception ex)
            {
                analysisStatus.Status = AnalysisStatusEnum.Failed;
                await UpdateAnalysisStatusAsync(analysisStatus);

                throw new Exception("Code analysis failed: " + ex.Message);
            }
        }

        // Läsbarhet (Readability) Score
        private int CalculateReadabilityScore(string code)
        {
            int score = 100;
            if (code.Split('\n').Length > 500) score -= 20;
            if (code.Contains("switch") && code.Contains("case")) score -= 10;
            if (code.Length > 1000) score -= 15; // Om koden är för lång
            // Additional readability checks
            if (code.Contains("todo")) score -= 5; // Check for TODO comments
            if (code.Contains("fixme")) score -= 5; // Check for FIXME comments

            return Math.Max(0, score);
        }

        // Säkerhet (Security) Score
        private int CalculateSecurityScore(string code)
        {
            int score = 100;
            if (code.Contains("SELECT * FROM")) score -= 20;
            if (code.Contains("<script>") || code.Contains("document.write")) score -= 25;
            if (code.Contains("HttpClient") && code.Contains(".GetAsync")) score -= 15;
            // Additional security checks
            if (code.Contains("eval")) score -= 30; // Avoid eval function, which can lead to code injection
            if (code.Contains("Thread.Sleep")) score -= 20; // Potential DoS attack

            return Math.Max(0, score);
        }

        // Prestanda (Performance) Score
        private int CalculatePerformanceScore(string code)
        {
            int score = 100;
            if (code.Contains("Thread.Sleep")) score -= 20;
            if (code.Contains("lock")) score -= 15;
            if (code.Contains(".GetAsync") && !code.Contains("await")) score -= 25;
            // Additional performance checks
            if (code.Contains("foreach") && code.Contains("List") && code.Contains("ToList")) score -= 10; // Avoid unnecessary ToList conversion

            return Math.Max(0, score);
        }

        private List<string> PerformSecurityAnalysis(string code)
        {
            var issues = new List<string>();

            if (code.Contains("SELECT * FROM"))
            {
                issues.Add("Potential SQL Injection vulnerability detected: Avoid using SELECT * in queries. Consider specifying columns to select.");
            }

            if (code.Contains("<script>") || code.Contains("document.write"))
            {
                issues.Add("Potential XSS vulnerability detected: Avoid directly injecting JavaScript.");
            }

            if (code.Contains("HttpClient") && code.Contains(".GetAsync"))
            {
                issues.Add("Potential insecure HTTP call detected: Use HTTPS and ensure proper validation.");
            }

            // Additional security vulnerabilities
            if (code.Contains("eval"))
            {
                issues.Add("Potential risk of code injection detected: Avoid using eval to execute code.");
            }

            if (code.Contains("Thread.Sleep"))
            {
                issues.Add("Potential denial of service (DoS) attack: Avoid using Thread.Sleep, consider using async/await.");
            }

            return issues;
        }

        private List<string> PerformPerformanceAnalysis(string code)
        {
            var issues = new List<string>();

            if (code.Contains("Thread.Sleep"))
            {
                issues.Add("Potential performance issue: Avoid using Thread.Sleep, consider using async/await.");
            }

            if (code.Contains("lock"))
            {
                issues.Add("Performance warning: Ensure locks are scoped appropriately to avoid deadlocks.");
            }

            // Additional performance issues
            if (code.Contains("foreach") && code.Contains("List") && code.Contains("ToList"))
            {
                issues.Add("Performance issue: Avoid unnecessary ToList conversions when iterating over a collection.");
            }

            return issues;
        }

        private List<string> AnalyzeReadability(string code)
        {
            var issues = new List<string>();

            if (code.Split('\n').Length > 500)
            {
                issues.Add("Code readability: File exceeds 500 lines. Consider breaking it into smaller modules.");
            }

            if (code.Contains("if") && !code.Contains("else"))
            {
                issues.Add("Code style: Unmatched if statements detected. Consider adding else or comments.");
            }

            if (code.Contains("switch") && code.Contains("case"))
            {
                issues.Add("Code style: Complex switch statements detected. Consider refactoring to improve readability.");
            }

            var functions = code.Split(new string[] { "public", "private", "protected" }, StringSplitOptions.None);
            foreach (var func in functions)
            {
                if (func.Length > 1000)
                {
                    issues.Add("Code readability: Function exceeds 1000 characters. Consider splitting into smaller functions.");
                }
            }

            // Additional readability issues
            if (code.Contains("todo"))
            {
                issues.Add("Code readability: TODO comments found, ensure they are addressed before finalizing.");
            }

            return issues;
        }

        private List<string> ParseESLintOutput(string eslintOutput)
        {
            return eslintOutput.Split('\n').Where(line => line.Contains("error")).ToList();
        }

        private int CalculateComplexityScore(string code)
        {
            return code.Length % 10; // A placeholder for complexity scoring, can be extended for more accurate analysis
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
    }
}
