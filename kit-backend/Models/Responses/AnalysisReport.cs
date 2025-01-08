using KitBackend.Models;
using System.ComponentModel.DataAnnotations;

public class AnalysisReport : BaseEntity
{
    [Key]
    public Guid ReportId { get; set; }
    public string BestPracticesFeedback { get; set; } = string.Empty;
    public int ComplexityScore { get; set; }
    public List<string> Issues { get; set; } = new List<string>();
}
