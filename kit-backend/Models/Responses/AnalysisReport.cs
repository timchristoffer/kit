using KitBackend.Models;
using System.ComponentModel.DataAnnotations;

public class AnalysisReport : BaseEntity
{
    [Key]
    public Guid ReportId { get; set; }
    public string BestPracticesFeedback { get; set; } = string.Empty;
    public int ComplexityScore { get; set; }
    public int ReadabilityScore { get; set; } 
    public int SecurityScore { get; set; } 
    public int PerformanceScore { get; set; } 
    public List<string> Issues { get; set; } = new List<string>();
    public List<string> SecurityIssues { get; set; } = new List<string>();
    public List<string> PerformanceIssues { get; set; } = new List<string>();
    public List<string> ReadabilityIssues { get; set; } = new List<string>();
    public string Explanation { get; set; } = string.Empty;
    public string PdfPath { get; set; } = string.Empty;
    public byte[]? PdfContent { get; set; }
}
