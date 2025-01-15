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

                // Generate analysis
                List<string> issues = RoslynAnalyzer.Analyze(code);

                if (code.Contains("import") || code.Contains("export"))
                {
                    var eslintReport = ESLintAnalyzer.Analyze(code);
                    issues.AddRange(ParseESLintOutput(eslintReport));
                }

                var securityIssues = PerformSecurityAnalysis(code);
                var performanceIssues = PerformPerformanceAnalysis(code);
                var readabilityIssues = AnalyzeReadability(code);

                int readabilityScore = CalculateReadabilityScore(code);
                int securityScore = CalculateSecurityScore(code);
                int performanceScore = CalculatePerformanceScore(code);
                int complexityScore = CalculateComplexityScore(code);

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

        // Readability Score
        private int CalculateReadabilityScore(string code)
        {
            int score = 100;
            if (code.Split('\n').Length > 500) score -= 20;
            if (code.Contains("switch") && code.Contains("case")) score -= 10;
            if (code.Length > 1000) score -= 15; 
            if (code.Contains("todo")) score -= 5; 
            if (code.Contains("fixme")) score -= 5; 
            if (code.Split('\n').Any(line => line.Length > 120)) score -= 10; 
            if (code.Count(c => c == '{') > 20) score -= 10;
            if (!code.Contains("//")) score -= 10;
            if (code.Contains("var ")) score -= 5; 
            if (code.Contains("magic number")) score -= 5;
            if (!code.Contains("PascalCase") || !code.Contains("camelCase")) score -= 5;
            if (code.Contains("if") && !code.Contains("else")) score -= 5; 
            if (code.Split(new string[] { "public", "private", "protected" }, StringSplitOptions.None).Any(func => func.Length > 1000)) score -= 10; // Check for long functions

            return Math.Max(0, score);
        }

        // Security Score
        private int CalculateSecurityScore(string code)
        {
            int score = 100;
            if (code.Contains("SELECT * FROM")) score -= 20;
            if (code.Contains("<script>") || code.Contains("document.write")) score -= 25;
            if (code.Contains("HttpClient") && code.Contains(".GetAsync")) score -= 15;
            if (code.Contains("eval")) score -= 30; 
            if (code.Contains("Thread.Sleep")) score -= 20; 
            if (code.Contains("password")) score -= 30; 
            if (code.Contains("private") && code.Contains("string") && code.Contains("=")) score -= 20; 
            if (code.Contains("MD5") || code.Contains("SHA1")) score -= 20; 
            if (code.Contains("catch") && !code.Contains("throw")) score -= 10;

            return Math.Max(0, score);
        }

        // Performance Score
        private int CalculatePerformanceScore(string code)
        {
            int score = 100;
            if (code.Contains("Thread.Sleep")) score -= 20;
            if (code.Contains("lock")) score -= 15;
            if (code.Contains(".GetAsync") && !code.Contains("await")) score -= 25;
            if (code.Contains("foreach") && code.Contains("List") && code.Contains("ToList")) score -= 10;
            if (code.Contains("new") && code.Contains("List") && code.Contains("Capacity")) score -= 10; 
            if (code.Contains("for") && code.Contains("Count")) score -= 10; 
            if (code.Contains("new") && code.Contains("object")) score -= 10;
            if (code.Contains("Dictionary") && code.Contains("new string")) score -= 15; 
            if (code.Contains("for") && code.Contains("for")) score -= 15; 
            if (code.Contains("Console.WriteLine") && code.Contains("for")) score -= 10; 
            if (code.Contains("List") && code.Contains("Add")) score -= 10; 
            if (code.Contains("Dictionary") && code.Contains("Add")) score -= 10; 

            return Math.Max(0, score);
        }

        // Complexity Score
        private int CalculateComplexityScore(string code)
        {
            int score = 100;
            int cyclomaticComplexity = CalculateCyclomaticComplexity(code);
            if (cyclomaticComplexity > 10) score -= 20; 
            if (code.Split(new string[] { "public", "private", "protected" }, StringSplitOptions.None).Length > 20) score -= 10; 
            if (code.Contains("class") && code.Contains(":")) score -= 10; 

            return Math.Max(0, score);
        }

        private int CalculateCyclomaticComplexity(string code)
        {
            int complexity = 1;
            complexity += code.Split(new string[] { "if", "else", "case", "for", "while", "&&", "||", "switch", "catch", "?", "do", "goto", "continue", "break", "function", "=>", "try", "finally" }, StringSplitOptions.None).Length - 1;
            return complexity;
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

            if (code.Contains("eval"))
            {
                issues.Add("Potential risk of code injection detected: Avoid using eval to execute code.");
            }

            if (code.Contains("Thread.Sleep"))
            {
                issues.Add("Potential denial of service (DoS) attack: Avoid using Thread.Sleep, consider using async/await.");
            }

            if (code.Contains("password"))
            {
                issues.Add("Security risk: Hardcoded password detected. Avoid hardcoding passwords in the code.");
            }

            if (code.Contains("private") && code.Contains("string") && code.Contains("="))
            {
                issues.Add("Security risk: Hardcoded sensitive data detected. Avoid hardcoding sensitive data in the code.");
            }

            if (code.Contains("MD5") || code.Contains("SHA1"))
            {
                issues.Add("Security risk: Insecure cryptographic practice detected. Avoid using MD5 or SHA1.");
            }

            if (code.Contains("catch") && !code.Contains("throw"))
            {
                issues.Add("Security risk: Improper error handling detected. Ensure exceptions are properly handled.");
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

            if (code.Contains("foreach") && code.Contains("List") && code.Contains("ToList"))
            {
                issues.Add("Performance issue: Avoid unnecessary ToList conversions when iterating over a collection.");
            }

            if (code.Contains("new") && code.Contains("List") && code.Contains("Capacity"))
            {
                issues.Add("Potential performance issue: Avoid excessive memory allocation by specifying an appropriate capacity for lists.");
            }

            if (code.Contains("for") && code.Contains("Count"))
            {
                issues.Add("Performance issue: Avoid inefficient loops by caching the count value.");
            }

            if (code.Contains("new") && code.Contains("object"))
            {
                issues.Add("Performance issue: Avoid unnecessary object creation.");
            }

            if (code.Contains("Dictionary") && code.Contains("new string"))
            {
                issues.Add("Performance issue: Excessive memory usage in dictionaries detected. Consider optimizing memory usage.");
            }

            if (code.Contains("for") && code.Contains("for"))
            {
                issues.Add("Performance issue: Nested loops detected. Consider optimizing the loop structure.");
            }

            if (code.Contains("Console.WriteLine") && code.Contains("for"))
            {
                issues.Add("Performance issue: Excessive console output in loops detected. Consider reducing console output.");
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

            if (code.Contains("todo"))
            {
                issues.Add("Code readability: TODO comments found, ensure they are addressed before finalizing.");
            }

            if (code.Split('\n').Any(line => line.Length > 120))
            {
                issues.Add("Code readability: Long lines detected. Consider breaking them into shorter lines.");
            }

            if (code.Count(c => c == '{') > 20)
            {
                issues.Add("Code readability: Deeply nested structures detected. Consider refactoring to improve readability.");
            }

            if (!code.Contains("//"))
            {
                issues.Add("Code readability: Lack of comments detected. Consider adding comments to improve readability.");
            }

            if (code.Contains("var "))
            {
                issues.Add("Code readability: Usage of var detected. Consider using explicit types for better readability.");
            }

            if (code.Contains("magic number"))
            {
                issues.Add("Code readability: Magic numbers detected. Consider defining constants for better readability.");
            }

            if (!code.Contains("PascalCase") || !code.Contains("camelCase"))
            {
                issues.Add("Code readability: Inconsistent naming conventions detected. Ensure consistent use of PascalCase and camelCase.");
            }

            return issues;
        }

        private List<string> ParseESLintOutput(string eslintOutput)
        {
            return eslintOutput.Split('\n').Where(line => line.Contains("error")).ToList();
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