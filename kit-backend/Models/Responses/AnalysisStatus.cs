using System;
using System.ComponentModel.DataAnnotations;

namespace KitBackend.Models.Responses
{
    public class AnalysisStatus : BaseEntity
    {
        [Key]
        public Guid AnalysisId { get; set; }
        public AnalysisStatusEnum Status { get; set; }
    }
    public enum AnalysisStatusEnum
    {
        Pending,
        InProgress,
        Completed,
        Failed

    }
}
