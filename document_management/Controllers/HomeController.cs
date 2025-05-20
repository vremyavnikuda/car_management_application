using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using document_management.Models;
using document_management.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using document_management.Models.ViewModels;

namespace document_management.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeViewModel
        {
            TotalDocuments = await _context.Documents.CountAsync(),
            RecentDocuments = await _context.Documents
                .OrderByDescending(d => d.CreatedAt)
                .Take(5)
                .Select(d => new DocumentSummaryViewModel
                {
                    Id = d.Id,
                    Title = d.Title,
                    CreatedAt = d.CreatedAt,
                    FileType = d.DocumentType
                })
                .ToListAsync(),
            DocumentTypes = await _context.Documents
                .GroupBy(d => d.DocumentType)
                .Select(g => new DocumentTypeSummary
                {
                    Type = g.Key,
                    Count = g.Count()
                })
                .ToListAsync()
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
