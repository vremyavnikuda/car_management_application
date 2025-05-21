using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using document_management.Data;
using document_management.Models;
using System.Security.Claims;
using document_management.Models.ViewModels;
using System.Text;
using System.Web;
using Aspose.Words;
using AsposeDocument = Aspose.Words.Document;

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
            if (document == null)
                return NotFound();
            if (document.DocumentType.ToLower() == "pdf" && document.FileContent != null)
                return File(document.FileContent, "application/pdf");
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
            if (document == null)
            {
                _logger.LogWarning("Attempt to preview non-existent document {DocumentId} by user {User}", id, User.Identity?.Name);
                return NotFound();
            }

            _logger.LogInformation("User {User} previewing document {DocumentId} ({Title})", User.Identity?.Name, document.Id, document.Title);

            switch (document.DocumentType.ToLower())
            {
                case "pdf":
                    if (document.FileContent != null)
                        return File(document.FileContent, "application/pdf");
                    if (!string.IsNullOrEmpty(document.FilePath) && System.IO.File.Exists(document.FilePath))
                        return File(await System.IO.File.ReadAllBytesAsync(document.FilePath), "application/pdf");
                    return NotFound();
                case "docx":
                    // Для DOCX просто редиректим на PreviewPdf (Aspose.Words)
                    return RedirectToAction("PreviewPdf", new { id });
                default:
                    return BadRequest($"Preview is not available for {document.DocumentType} documents");
            }
        }

        private async Task UpdateDocumentContent(document_management.Models.Document document)
        {
            if (document.FileContent == null && !string.IsNullOrEmpty(document.FilePath) && System.IO.File.Exists(document.FilePath))
            {
                _logger.LogInformation("Updating document {DocumentId} content from file {FilePath}", 
                    document.Id, document.FilePath);

                try
                {
                    document.FileContent = await System.IO.File.ReadAllBytesAsync(document.FilePath);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Document {DocumentId} content updated successfully, size: {Size} bytes", 
                        document.Id, document.FileContent.Length);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating document {DocumentId} content", document.Id);
                }
            }
        }

        // --- PDF Preview for DOCX using Aspose.Words ---
        public async Task<IActionResult> PreviewPdf(int id)
        {
            _logger.LogInformation("PreviewPdf called for document {DocumentId}", id);

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                _logger.LogWarning("Document {DocumentId} not found", id);
                return NotFound($"Document {id} not found");
            }

            // Try to update document content if missing
            await UpdateDocumentContent(document);

            _logger.LogInformation("Document found: Id={Id}, Type={Type}, HasContent={HasContent}", 
                document.Id, 
                document.DocumentType,
                document.FileContent != null);

            if (document.FileContent == null || document.FileContent.Length == 0)
            {
                _logger.LogWarning("Document {DocumentId} has no content", id);
                return NotFound($"Document {id} has no content");
            }

            try
            {
                if (document.DocumentType.ToLower() == "pdf")
                {
                    _logger.LogInformation("Serving PDF document directly, size: {Size} bytes", document.FileContent.Length);
                    return File(document.FileContent, "application/pdf");
                }
                else if (document.DocumentType.ToLower() == "docx")
                {
                    _logger.LogInformation("Converting DOCX to PDF, input size: {Size} bytes", document.FileContent.Length);
                    using (var inputStream = new MemoryStream(document.FileContent))
                    using (var outputStream = new MemoryStream())
                    {
                        var doc = new AsposeDocument(inputStream);
                        doc.Save(outputStream, Aspose.Words.SaveFormat.Pdf);
                        _logger.LogInformation("DOCX successfully converted to PDF, output size: {Size} bytes", outputStream.Length);
                        return File(outputStream.ToArray(), "application/pdf");
                    }
                }
                else
                {
                    _logger.LogWarning("Unsupported document type: {DocumentType}", document.DocumentType);
                    return BadRequest($"Unsupported file type: {document.DocumentType}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing document {DocumentId}", id);
                return StatusCode(500, $"Error processing document: {ex.Message}");
            }
        }

        // Add a method to update all documents
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAllDocumentsContent()
        {
            var documents = await _context.Documents.ToListAsync();
            var updatedCount = 0;

            foreach (document_management.Models.Document document in documents)
            {
                if (document.FileContent == null && !string.IsNullOrEmpty(document.FilePath) && System.IO.File.Exists(document.FilePath))
                {
                    try
                    {
                        document.FileContent = await System.IO.File.ReadAllBytesAsync(document.FilePath);
                        updatedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating document {DocumentId} content", document.Id);
                    }
                }
            }

            if (updatedCount > 0)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated content for {Count} documents", updatedCount);
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 