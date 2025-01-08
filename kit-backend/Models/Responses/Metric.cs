using System;
using System.ComponentModel.DataAnnotations;

namespace KitBackend.Models.Responses
{
    public class Metric : BaseEntity
    {
        [Key]
        public Guid MetricId { get; set; }
        public Guid AnalysisId { get; set; }
        public string MetricName { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
