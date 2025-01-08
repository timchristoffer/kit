using KitBackend.Models;
using System.ComponentModel.DataAnnotations;

public class AnalysisReport : BaseEntity
{
    [Key]
    public Guid ReportId { get; set; }
    public string BestPracticesFeedback { get; set; } = string.Empty;
    public int ComplexityScore { get; set; }
    public int ReadabilityScore { get; set; } // Ny egenskap för readability poäng
    public int SecurityScore { get; set; } // Ny egenskap för security poäng
    public int PerformanceScore { get; set; } // Ny egenskap för performance poäng
    public List<string> Issues { get; set; } = new List<string>();
    public List<string> SecurityIssues { get; set; } = new List<string>();
    public List<string> PerformanceIssues { get; set; } = new List<string>();
    public List<string> ReadabilityIssues { get; set; } = new List<string>();
}
