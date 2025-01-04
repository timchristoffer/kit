public class AnalysisRequest : BaseEntity
{
    public Guid RequestId { get; set; } // Unikt ID för varje förfrågan
    public string SourceType { get; set; } = string.Empty; // "File" eller "Snippet"
    public string Content { get; set; } = string.Empty; // Kod eller filinnehåll i textformat
}