using System;
using System.ComponentModel.DataAnnotations;

namespace KitBackend.Models.Data
{
    public class CodeSnippet : BaseEntity
    {
        [Key]
        public Guid SnippetId { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = string.Empty;
    }
}