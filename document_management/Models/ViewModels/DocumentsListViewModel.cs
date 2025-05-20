using document_management.Models;

namespace document_management.Models.ViewModels
{
    public class DocumentsListViewModel
    {
        public required IEnumerable<Document> Documents { get; set; }
        public bool HasDocuments { get; set; }
    }
} 