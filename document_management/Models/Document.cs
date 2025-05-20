using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace document_management.Models
{
    public class Document
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        [Required]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = string.Empty;

        public string? FilePath { get; set; }

        public string? ContentType { get; set; }

        public virtual ICollection<DocumentVersion> Versions { get; set; }

        public Document()
        {
            CreatedAt = DateTime.UtcNow;
            Versions = new List<DocumentVersion>();
        }
    }
} 