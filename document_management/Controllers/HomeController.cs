using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using document_management.Models;
using document_management.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using document_management.Models.ViewModels;
using document_management.Services;
using System.Security.Claims;

namespace document_management.Controllers;

public class HomeController : Controller
{
    private readonly ILoggingService _loggingService;
    private readonly ApplicationDbContext _context;

    public HomeController(ILoggingService loggingService, ApplicationDbContext context)
    {
        _loggingService = loggingService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.Identity?.IsAuthenticated == true ? 
            User.FindFirstValue(ClaimTypes.NameIdentifier) : "anonymous";
        
        _loggingService.LogUserAction(userId, "HomePage", "User accessed home page");

        try
        {
            var totalDocuments = await _context.Documents.CountAsync();
            var recentDocuments = await _context.Documents
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

            var documentTypes = await _context.Documents
                .GroupBy(d => d.DocumentType)
                .Select(g => new DocumentTypeSummary
                {
                    Type = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                TotalDocuments = totalDocuments,
                RecentDocuments = recentDocuments,
                DocumentTypes = documentTypes
            };

            _loggingService.LogSystemEvent("HomePageStats", 
                $"Home page loaded with {totalDocuments} total documents, {recentDocuments.Count} recent documents, {documentTypes.Count} document types");

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, userId, "HomePageLoad", "Error loading home page data");
            throw;
        }
    }

    public IActionResult Privacy()
    {
        var userId = User.Identity?.IsAuthenticated == true ? 
            User.FindFirstValue(ClaimTypes.NameIdentifier) : "anonymous";
        
        _loggingService.LogUserAction(userId, "PrivacyPage", "User accessed privacy page");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var userId = User.Identity?.IsAuthenticated == true ? 
            User.FindFirstValue(ClaimTypes.NameIdentifier) : "anonymous";
        
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _loggingService.LogError(
            new Exception($"Error page accessed with RequestId: {requestId}"), 
            userId, 
            "ErrorPage", 
            $"User accessed error page, RequestId: {requestId}");
            
        return View(new ErrorViewModel { RequestId = requestId });
    }
}
