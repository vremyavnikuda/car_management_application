using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using document_management.Data;
using document_management.Models;
using System.Security.Claims;
using document_management.Models.ViewModels;

namespace document_management.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(ApplicationDbContext context, ILogger<DocumentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("User {User} accessed documents list", User.Identity?.Name);
            
            var documents = await _context.Documents
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            var viewModel = new DocumentsListViewModel
            {
                Documents = documents,
                HasDocuments = documents.Any()
            };

            _logger.LogInformation("User {User} has {DocumentCount} documents", 
                User.Identity?.Name, documents.Count);

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var document = await _context.Documents
                .Include(d => d.Versions.OrderByDescending(v => v.ModifiedAt))
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
            {
                _logger.LogWarning("Document {DocumentId} not found when accessed by user {User}", id, User.Identity?.Name);
                return NotFound();
            }

            _logger.LogInformation("User {User} viewed document {DocumentId} ({Title})", 
                User.Identity?.Name, document.Id, document.Title);
            return View(document);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                _logger.LogWarning("Attempt to delete non-existent document {DocumentId} by user {User}", 
                    id, User.Identity?.Name);
                return NotFound();
            }

            _logger.LogInformation("User {User} initiated deletion of document {DocumentId} ({Title})", 
                User.Identity?.Name, document.Id, document.Title);

            // Удаляем физический файл
            if (!string.IsNullOrEmpty(document.FilePath) && System.IO.File.Exists(document.FilePath))
            {
                try
                {
                    System.IO.File.Delete(document.FilePath);
                    _logger.LogInformation("Physical file deleted: {FilePath}", document.FilePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting physical file {FilePath}", document.FilePath);
                }
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Document {DocumentId} ({Title}) successfully deleted by user {User}", 
                document.Id, document.Title, User.Identity?.Name);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDocument(int id, string title, string status, IFormFile? file)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                _logger.LogWarning("Attempt to update non-existent document {DocumentId} by user {User}", 
                    id, User.Identity?.Name);
                return NotFound();
            }

            _logger.LogInformation("User {User} updating document {DocumentId} ({Title})", 
                User.Identity?.Name, document.Id, document.Title);

            // Обновляем основные данные
            document.Title = title;
            document.Status = status;

            // Если загружен новый файл
            if (file != null)
            {
                _logger.LogInformation("New file upload for document {DocumentId}: {FileName} ({ContentType}, {FileSize} bytes)", 
                    document.Id, file.FileName, file.ContentType, file.Length);

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    _logger.LogInformation("Created uploads directory: {UploadsFolder}", uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    _logger.LogInformation("File successfully saved to {FilePath}", filePath);

                    // Создаем новую версию
                    var version = new DocumentVersion
                    {
                        DocumentId = document.Id,
                        VersionNumber = (document.Versions.Count + 1).ToString(),
                        FilePath = filePath,
                        ModifiedBy = User.FindFirstValue(ClaimTypes.Email) ?? "Unknown",
                        ModifiedAt = DateTime.UtcNow,
                        ChangeDescription = "Updated document and file"
                    };

                    document.Versions.Add(version);
                    document.FilePath = filePath;
                    document.ContentType = file.ContentType;

                    _logger.LogInformation("New version {VersionNumber} created for document {DocumentId}", 
                        version.VersionNumber, document.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving uploaded file for document {DocumentId}", document.Id);
                    throw;
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Document {DocumentId} successfully updated by user {User}", 
                document.Id, User.Identity?.Name);

            return RedirectToAction(nameof(Details), new { id = document.Id });
        }

        public async Task<IActionResult> Download(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null || string.IsNullOrEmpty(document.FilePath) || !System.IO.File.Exists(document.FilePath))
            {
                _logger.LogWarning("Attempt to download non-existent or missing file for document {DocumentId} by user {User}", 
                    id, User.Identity?.Name);
                return NotFound();
            }

            _logger.LogInformation("User {User} downloading document {DocumentId} ({Title})", 
                User.Identity?.Name, document.Id, document.Title);

            try
            {
                var memory = await System.IO.File.ReadAllBytesAsync(document.FilePath);
                _logger.LogInformation("Document {DocumentId} successfully downloaded ({FileSize} bytes)", 
                    document.Id, memory.Length);
                return File(memory, document.ContentType ?? "application/octet-stream", Path.GetFileName(document.FilePath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading document {DocumentId}", document.Id);
                throw;
            }
        }

        public async Task<IActionResult> DownloadVersion(int versionId)
        {
            var version = await _context.DocumentVersions.FindAsync(versionId);
            if (version == null || string.IsNullOrEmpty(version.FilePath) || !System.IO.File.Exists(version.FilePath))
            {
                _logger.LogWarning("Attempt to download non-existent or missing version {VersionId} by user {User}", 
                    versionId, User.Identity?.Name);
                return NotFound();
            }

            _logger.LogInformation("User {User} downloading version {VersionId} of document {DocumentId}", 
                User.Identity?.Name, version.Id, version.DocumentId);

            try
            {
                var memory = await System.IO.File.ReadAllBytesAsync(version.FilePath);
                _logger.LogInformation("Version {VersionId} successfully downloaded ({FileSize} bytes)", 
                    version.Id, memory.Length);
                return File(memory, "application/octet-stream", Path.GetFileName(version.FilePath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading version {VersionId}", version.Id);
                throw;
            }
        }

        public async Task<IActionResult> Preview(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null || string.IsNullOrEmpty(document.FilePath) || !System.IO.File.Exists(document.FilePath))
            {
                _logger.LogWarning("Attempt to preview non-existent or missing file for document {DocumentId} by user {User}", 
                    id, User.Identity?.Name);
                return NotFound();
            }

            if (!document.DocumentType.Equals("PDF", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Attempt to preview non-PDF document {DocumentId} by user {User}", 
                    id, User.Identity?.Name);
                return BadRequest("Preview is only available for PDF documents");
            }

            _logger.LogInformation("User {User} previewing document {DocumentId} ({Title})", 
                User.Identity?.Name, document.Id, document.Title);

            try
            {
                var memory = await System.IO.File.ReadAllBytesAsync(document.FilePath);
                _logger.LogInformation("Document {DocumentId} successfully loaded for preview ({FileSize} bytes)", 
                    document.Id, memory.Length);
                return File(memory, "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing document {DocumentId}", document.Id);
                throw;
            }
        }
    }
} 