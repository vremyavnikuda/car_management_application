using System;
using System.ComponentModel.DataAnnotations;

namespace document_management.Models
{
    public class DocumentVersion
    {
        public int Id { get; set; }

        public int DocumentId { get; set; }
        public virtual Document Document { get; set; } = null!;

        [Required]
        public string VersionNumber { get; set; } = string.Empty;

        public string? FilePath { get; set; }

        [Required]
        public string ModifiedBy { get; set; } = string.Empty;

        public DateTime ModifiedAt { get; set; }

        public string? ChangeDescription { get; set; }

        public DocumentVersion()
        {
            ModifiedAt = DateTime.UtcNow;
        }
    }
} 