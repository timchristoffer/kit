using System;
using System.ComponentModel.DataAnnotations;

namespace KitBackend.Models.Data
{
    public class UploadedFile : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; } = DateTime.UtcNow; // Tid i UTC
        public long FileSize { get; set; }
        public string? Uploader { get; set; }
        public byte[] FileContent { get; set; } = Array.Empty<byte>();

        // FK till Project
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = new Project();
    }
}
