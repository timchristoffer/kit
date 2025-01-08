using KitBackend.Models.Responses;
using KitBackend.Services;

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

        // Spara initial status
        await SaveAnalysisStatusAsync(analysisStatus);

        try
        {
            // Uppdatera status till InProgress
            analysisStatus.Status = AnalysisStatusEnum.InProgress;
            await UpdateAnalysisStatusAsync(analysisStatus);

            // Första steget: Analysera koden (val av analyser beroende på typ av kod)
            List<string> issues = RoslynAnalyzer.Analyze(code); // Använd Roslyn för C#-kod

            // Om det är JavaScript eller TypeScript, använd ESLint
            if (code.Contains("import") || code.Contains("export"))
            {
                var eslintReport = ESLintAnalyzer.Analyze(code);
                issues.AddRange(ParseESLintOutput(eslintReport));
            }

            // Skapa och spara rapport
            var report = new AnalysisReport
            {
                ReportId = Guid.NewGuid(),
                Issues = issues,
                ComplexityScore = CalculateComplexityScore(code),
                BestPracticesFeedback = "Ensure your code follows best practices."
            };

            // Spara rapporten direkt i databasen
            await _context.AnalysisReport.AddAsync(report);
            await _context.SaveChangesAsync();

            // Uppdatera status till Completed
            analysisStatus.Status = AnalysisStatusEnum.Completed;
            await UpdateAnalysisStatusAsync(analysisStatus);

            return report;
        }
        catch (Exception ex)
        {
            // Uppdatera status till Failed vid fel
            analysisStatus.Status = AnalysisStatusEnum.Failed;
            await UpdateAnalysisStatusAsync(analysisStatus);

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
        // Parsar ESLint output och extraherar viktiga problem
        return eslintOutput.Split('\n').Where(line => line.Contains("error")).ToList();
    }

    private int CalculateComplexityScore(string code)
    {
        // Här kan du skapa en funktion för att beräkna kodens komplexitet
        return code.Length % 10; // Exempel på enkel beräkning
    }
}

