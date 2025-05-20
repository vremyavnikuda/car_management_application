using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using document_management.Data;
using document_management.Models;
using document_management.Models.ViewModels;
using System.Security.Claims;

namespace document_management.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(ApplicationDbContext context, ILogger<ProfileController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound();
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        // Получаем все документы, созданные пользователем
        var createdDocuments = await _context.Documents
            .Where(d => d.Author == user.Email)
            .OrderByDescending(d => d.CreatedAt)
            .Take(5)
            .Select(d => new DocumentSummaryViewModel
            {
                Id = d.Id,
                Title = d.Title,
                CreatedAt = d.CreatedAt,
                FileType = d.DocumentType
            })
            .ToListAsync();

        // Получаем все документы, отредактированные пользователем
        var editedDocuments = await _context.DocumentVersions
            .Where(v => v.ModifiedBy == user.Email)
            .Select(v => new DocumentSummaryViewModel
            {
                Id = v.DocumentId,
                Title = v.Document.Title,
                CreatedAt = v.ModifiedAt,
                FileType = v.Document.DocumentType
            })
            .Distinct()
            .OrderByDescending(d => d.CreatedAt)
            .Take(5)
            .ToListAsync();

        // Получаем последние активности
        var recentActivities = await _context.DocumentVersions
            .Where(v => v.ModifiedBy == user.Email)
            .OrderByDescending(v => v.ModifiedAt)
            .Take(10)
            .Select(v => new DocumentActivityViewModel
            {
                DocumentId = v.DocumentId,
                DocumentTitle = v.Document.Title,
                ActivityType = "Edited",
                ActivityDate = v.ModifiedAt,
                ChangeDescription = v.ChangeDescription
            })
            .ToListAsync();

        // Добавляем создание документов в активность
        var creationActivities = await _context.Documents
            .Where(d => d.Author == user.Email)
            .OrderByDescending(d => d.CreatedAt)
            .Take(10)
            .Select(d => new DocumentActivityViewModel
            {
                DocumentId = d.Id,
                DocumentTitle = d.Title,
                ActivityType = "Created",
                ActivityDate = d.CreatedAt,
                ChangeDescription = "Document created"
            })
            .ToListAsync();

        recentActivities.AddRange(creationActivities);
        recentActivities = recentActivities
            .OrderByDescending(a => a.ActivityDate)
            .Take(10)
            .ToList();

        var viewModel = new ProfileViewModel
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            RegistrationDate = DateTime.UtcNow,
            TotalDocuments = await _context.Documents.CountAsync(d => d.Author == user.Email),
            RecentActivities = recentActivities,
            CreatedDocuments = createdDocuments,
            EditedDocuments = editedDocuments
        };

        // Try to get the user's first document creation date as a proxy for registration date
        var firstDocument = await _context.Documents
            .Where(d => d.Author == user.Email)
            .OrderBy(d => d.CreatedAt)
            .FirstOrDefaultAsync();

        if (firstDocument != null)
        {
            viewModel.RegistrationDate = firstDocument.CreatedAt;
        }

        return View(viewModel);
    }
} 