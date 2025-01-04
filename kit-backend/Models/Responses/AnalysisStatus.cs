public class AnalysisStatus : BaseEntity
{
    public Guid AnalysisId { get; set; }
    public AnalysisStatusEnum Status { get; set; } // Enum används istället för string
}

// AnalysisStatusEnum.cs
public enum AnalysisStatusEnum
{
    Pending,
    InProgress,
    Completed,
    Failed
}