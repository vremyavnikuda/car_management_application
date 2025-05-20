using System;
using System.Collections.Generic;

namespace document_management.Models.ViewModels
{
    public class ProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int TotalDocuments { get; set; }
        public List<DocumentActivityViewModel> RecentActivities { get; set; } = new();
        public List<DocumentSummaryViewModel> CreatedDocuments { get; set; } = new();
        public List<DocumentSummaryViewModel> EditedDocuments { get; set; } = new();
    }

    public class DocumentActivityViewModel
    {
        public int DocumentId { get; set; }
        public string DocumentTitle { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty; // "Created" или "Edited"
        public DateTime ActivityDate { get; set; }
        public string? ChangeDescription { get; set; }
    }
} 