public class CodeAnalysisResult : BaseEntity
{
    public int Id { get; set; }
    public int FileId { get; set; }
    public DateTime AnalysisDate { get; set; } = DateTime.UtcNow;
    public int Warnings { get; set; }
    public int Errors { get; set; }
    public string Summary { get; set; } = string.Empty;
}