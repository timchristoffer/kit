using System;
using System.ComponentModel.DataAnnotations;

namespace KitBackend.Models.Data
{
    public class ErrorLog : BaseEntity
    {
        [Key]
        public Guid LogId { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;
        public string AdditionalInfo { get; set; } = string.Empty;
    }
}