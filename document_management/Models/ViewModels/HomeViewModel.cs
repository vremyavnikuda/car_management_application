using System;
using System.Collections.Generic;

namespace document_management.Models.ViewModels
{
    public class HomeViewModel
    {
        public int TotalDocuments { get; set; }
        public List<DocumentSummaryViewModel> RecentDocuments { get; set; } = new();
        public List<DocumentTypeSummary> DocumentTypes { get; set; } = new();
    }

    public class DocumentSummaryViewModel
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string FileType { get; set; }
    }

    public class DocumentTypeSummary
    {
        public required string Type { get; set; }
        public int Count { get; set; }
    }
} 