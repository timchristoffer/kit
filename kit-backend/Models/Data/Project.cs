using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace KitBackend.Models.Data
{
    public class Project : BaseEntity
    {
        [Key]
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Samling av UploadedFiles
        public List<UploadedFile> UploadedFiles { get; set; } = new List<UploadedFile>();

        public Guid UserId { get; set; }
        public User User { get; set; } = new User();
    }
}
