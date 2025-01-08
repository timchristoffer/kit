using System;
using System.ComponentModel.DataAnnotations;

namespace KitBackend.Models.Data
{
    public class User : BaseEntity
    {
        [Key]
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
