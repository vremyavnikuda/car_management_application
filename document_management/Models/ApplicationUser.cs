using Microsoft.AspNetCore.Identity;

namespace document_management.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
} 