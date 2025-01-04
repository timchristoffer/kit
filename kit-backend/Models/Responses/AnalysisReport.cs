public class AnalysisReport : BaseEntity
{
    public Guid ReportId { get; set; } // Unikt ID för rapporten
    public Guid? FileId { get; set; } // Koppling till uppladdad fil (om tillämpligt)
    public Guid? SnippetId { get; set; } // Koppling till kodblock (om tillämpligt)
    public string BestPracticesFeedback { get; set; } = string.Empty; // Feedback angående best practices
    public int ComplexityScore { get; set; } // T.ex. ett värde för kodens komplexitet
    public List<string> Issues { get; set; } = new List<string>(); // Lista över problem eller varningar
}