public class Metric : BaseEntity
{
    public Guid MetricId { get; set; }
    public Guid AnalysisId { get; set; }
    public string MetricName { get; set; } = string.Empty; // e.g., "Code Lines", "Warnings", "Errors"
    public int Value { get; set; }
}