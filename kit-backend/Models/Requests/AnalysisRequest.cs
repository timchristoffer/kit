using System;
using System.ComponentModel.DataAnnotations;

namespace KitBackend.Models.Requests
{
    public class AnalysisRequest
    {
        [Key]
        public Guid RequestId { get; set; }
        public string? SourceType { get; set; }
        public string? Content { get; set; } 
        public int? FileId { get; set; }

    }

}