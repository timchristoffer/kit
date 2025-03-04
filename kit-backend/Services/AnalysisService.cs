using KitBackend.Models.Responses;
using KitBackend.Models.Data;
using KitBackend.Services;
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

                var securityIssues = PerformSecurityAnalysis(code);
                var performanceIssues = PerformPerformanceAnalysis(code);
                var readabilityIssues = AnalyzeReadability(code);

                int readabilityScore = CalculateReadabilityScore(code);
                int securityScore = CalculateSecurityScore(code);
                int performanceScore = CalculatePerformanceScore(code);
                int complexityScore = CalculateComplexityScore(code);

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
                    _logger.LogInformation("Generating PDF report for analysis {ReportId}", report.ReportId);
                    var (pdfPath, pdfContent) = _pdfReportService.GeneratePdfReport(report);
                    report.PdfPath = pdfPath;
                    report.PdfContent = pdfContent; // Spara PDF-innehållet direkt i databasen
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to generate PDF report for ReportId: {ReportId}. Analysis will continue without PDF.", report.ReportId);
                    // Fortsätt utan att felja helt på grund av PDF-fel
                    report.PdfPath = "PDF generation failed: " + ex.Message;
                    report.PdfContent = null; // Sätt explicit till null för tydlighets skull
                    await _context.SaveChangesAsync();

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

        public async Task UpdateReportAsync(AnalysisReport report)
        {
            _context.AnalysisReport.Update(report);
            await _context.SaveChangesAsync();
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
            if (code.Contains("goto")) score -= 10;
            if (code.Contains("continue")) score -= 5;
            if (code.Contains("break")) score -= 5;
            if (code.Contains("try") && !code.Contains("catch")) score -= 10;
            if (code.Contains("catch") && !code.Contains("finally")) score -= 5;
            if (code.Contains("while") && !code.Contains("do")) score -= 5;
            if (code.Contains("do") && !code.Contains("while")) score -= 5;

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
            if (code.Contains("HttpClient") && !code.Contains("Dispose")) score -= 10;
            if (code.Contains("File") && code.Contains(".ReadAllText")) score -= 15;
            if (code.Contains("File") && code.Contains(".WriteAllText")) score -= 15;
            if (code.Contains("Directory") && code.Contains(".CreateDirectory")) score -= 10;
            if (code.Contains("Directory") && code.Contains(".Delete")) score -= 10;

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
            if (code.Contains("Array") && code.Contains(".Resize")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Copy")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Sort")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Reverse")) score -= 10;
            if (code.Contains("Array") && code.Contains(".IndexOf")) score -= 10;
            if (code.Contains("Array") && code.Contains(".LastIndexOf")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Find")) score -= 10;
            if (code.Contains("Array") && code.Contains(".FindAll")) score -= 10;
            if (code.Contains("Array") && code.Contains(".FindIndex")) score -= 10;
            if (code.Contains("Array") && code.Contains(".FindLast")) score -= 10;
            if (code.Contains("Array") && code.Contains(".FindLastIndex")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Exists")) score -= 10;
            if (code.Contains("Array") && code.Contains(".TrueForAll")) score -= 10;
            if (code.Contains("Array") && code.Contains(".ForEach")) score -= 10;
            if (code.Contains("Array") && code.Contains(".ConvertAll")) score -= 10;
            if (code.Contains("Array") && code.Contains(".BinarySearch")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Clear")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Clone")) score -= 10;
            if (code.Contains("Array") && code.Contains(".CopyTo")) score -= 10;
            if (code.Contains("Array") && code.Contains(".GetEnumerator")) score -= 10;
            if (code.Contains("Array") && code.Contains(".GetLength")) score -= 10;
            if (code.Contains("Array") && code.Contains(".GetLongLength")) score -= 10;
            if (code.Contains("Array") && code.Contains(".GetLowerBound")) score -= 10;
            if (code.Contains("Array") && code.Contains(".GetUpperBound")) score -= 10;
            if (code.Contains("Array") && code.Contains(".Initialize")) score -= 10;
            if (code.Contains("Array") && code.Contains(".SetValue")) score -= 10;
            if (code.Contains("Array") && code.Contains(".GetValue")) score -= 10;

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
            if (code.Contains("interface")) score -= 10;
            if (code.Contains("abstract")) score -= 10;
            if (code.Contains("virtual")) score -= 10;
            if (code.Contains("override")) score -= 10;
            if (code.Contains("sealed")) score -= 10;
            if (code.Contains("static")) score -= 10;
            if (code.Contains("readonly")) score -= 10;
            if (code.Contains("const")) score -= 10;
            if (code.Contains("enum")) score -= 10;
            if (code.Contains("struct")) score -= 10;
            if (code.Contains("delegate")) score -= 10;
            if (code.Contains("event")) score -= 10;
            if (code.Contains("operator")) score -= 10;
            if (code.Contains("implicit")) score -= 10;
            if (code.Contains("explicit")) score -= 10;
            if (code.Contains("params")) score -= 10;
            if (code.Contains("ref")) score -= 10;
            if (code.Contains("out")) score -= 10;
            if (code.Contains("in")) score -= 10;
            if (code.Contains("yield")) score -= 10;
            if (code.Contains("async")) score -= 10;
            if (code.Contains("await")) score -= 10;
            if (code.Contains("lock")) score -= 10;
            if (code.Contains("using")) score -= 10;
            if (code.Contains("fixed")) score -= 10;
            if (code.Contains("unsafe")) score -= 10;
            if (code.Contains("checked")) score -= 10;
            if (code.Contains("unchecked")) score -= 10;
            if (code.Contains("sizeof")) score -= 10;
            if (code.Contains("typeof")) score -= 10;
            if (code.Contains("nameof")) score -= 10;
            if (code.Contains("default")) score -= 10;
            if (code.Contains("switch")) score -= 10;
            if (code.Contains("case")) score -= 10;
            if (code.Contains("goto")) score -= 10;
            if (code.Contains("continue")) score -= 10;
            if (code.Contains("break")) score -= 10;
            if (code.Contains("try")) score -= 10;
            if (code.Contains("catch")) score -= 10;
            if (code.Contains("finally")) score -= 10;
            if (code.Contains("throw")) score -= 10;
            if (code.Contains("return")) score -= 10;
            if (code.Contains("yield")) score -= 10;
            if (code.Contains("await")) score -= 10;
            if (code.Contains("async")) score -= 10;
            if (code.Contains("lock")) score -= 10;
            if (code.Contains("using")) score -= 10;
            if (code.Contains("fixed")) score -= 10;
            if (code.Contains("unsafe")) score -= 10;
            if (code.Contains("checked")) score -= 10;
            if (code.Contains("unchecked")) score -= 10;
            if (code.Contains("sizeof")) score -= 10;
            if (code.Contains("typeof")) score -= 10;
            if (code.Contains("nameof")) score -= 10;
            if (code.Contains("default")) score -= 10;

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

            if (code.Contains("HttpClient") && !code.Contains("Dispose"))
            {
                issues.Add("Security risk: HttpClient instance not disposed properly.");
            }

            if (code.Contains("File") && code.Contains(".ReadAllText"))
            {
                issues.Add("Security risk: Potential file read vulnerability detected.");
            }

            if (code.Contains("File") && code.Contains(".WriteAllText"))
            {
                issues.Add("Security risk: Potential file write vulnerability detected.");
            }

            if (code.Contains("Directory") && code.Contains(".CreateDirectory"))
            {
                issues.Add("Security risk: Potential directory creation vulnerability detected.");
            }

            if (code.Contains("Directory") && code.Contains(".Delete"))
            {
                issues.Add("Security risk: Potential directory deletion vulnerability detected.");
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

            if (code.Contains("Array") && code.Contains(".Resize"))
            {
                issues.Add("Performance issue: Avoid using Array.Resize, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".Copy"))
            {
                issues.Add("Performance issue: Avoid using Array.Copy, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".Sort"))
            {
                issues.Add("Performance issue: Avoid using Array.Sort, consider using a more efficient sorting algorithm.");
            }

            if (code.Contains("Array") && code.Contains(".Reverse"))
            {
                issues.Add("Performance issue: Avoid using Array.Reverse, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".IndexOf"))
            {
                issues.Add("Performance issue: Avoid using Array.IndexOf, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".LastIndexOf"))
            {
                issues.Add("Performance issue: Avoid using Array.LastIndexOf, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".Find"))
            {
                issues.Add("Performance issue: Avoid using Array.Find, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".FindAll"))
            {
                issues.Add("Performance issue: Avoid using Array.FindAll, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".FindIndex"))
            {
                issues.Add("Performance issue: Avoid using Array.FindIndex, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".FindLast"))
            {
                issues.Add("Performance issue: Avoid using Array.FindLast, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".FindLastIndex"))
            {
                issues.Add("Performance issue: Avoid using Array.FindLastIndex, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".Exists"))
            {
                issues.Add("Performance issue: Avoid using Array.Exists, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".TrueForAll"))
            {
                issues.Add("Performance issue: Avoid using Array.TrueForAll, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".ForEach"))
            {
                issues.Add("Performance issue: Avoid using Array.ForEach, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".ConvertAll"))
            {
                issues.Add("Performance issue: Avoid using Array.ConvertAll, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".BinarySearch"))
            {
                issues.Add("Performance issue: Avoid using Array.BinarySearch, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".Clear"))
            {
                issues.Add("Performance issue: Avoid using Array.Clear, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".Clone"))
            {
                issues.Add("Performance issue: Avoid using Array.Clone, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".CopyTo"))
            {
                issues.Add("Performance issue: Avoid using Array.CopyTo, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".GetEnumerator"))
            {
                issues.Add("Performance issue: Avoid using Array.GetEnumerator, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".GetLength"))
            {
                issues.Add("Performance issue: Avoid using Array.GetLength, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".GetLongLength"))
            {
                issues.Add("Performance issue: Avoid using Array.GetLongLength, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".GetLowerBound"))
            {
                issues.Add("Performance issue: Avoid using Array.GetLowerBound, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".GetUpperBound"))
            {
                issues.Add("Performance issue: Avoid using Array.GetUpperBound, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".Initialize"))
            {
                issues.Add("Performance issue: Avoid using Array.Initialize, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".SetValue"))
            {
                issues.Add("Performance issue: Avoid using Array.SetValue, consider using a more efficient data structure.");
            }

            if (code.Contains("Array") && code.Contains(".GetValue"))
            {
                issues.Add("Performance issue: Avoid using Array.GetValue, consider using a more efficient data structure.");
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
    }
}