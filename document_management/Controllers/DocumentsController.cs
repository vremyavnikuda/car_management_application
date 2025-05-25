using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using document_management.Data;
using document_management.Models;
using System.Security.Claims;
using document_management.Models.ViewModels;
using AsposeDocument = Aspose.Words.Document;
using ModelDocument = document_management.Models.Document;
using document_management.Services;

namespace document_management.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoggingService _loggingService;

        public DocumentsController(
            ApplicationDbContext context,
            ILoggingService loggingService)
        {
            _context = context;
            _loggingService = loggingService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _loggingService.LogUserAction(userId, "ViewDocumentsList", "Accessing documents list");
            
            var documents = await _context.Documents
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            var viewModel = new DocumentsListViewModel
            {
                Documents = documents,
                HasDocuments = documents.Any()
            };

            _loggingService.LogUserAction(userId, "ViewDocumentsList", $"Found {documents.Count} documents");

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _loggingService.LogDocumentOperation(userId, id, "ViewDetails", "Accessing document details");

            var document = await _context.Documents
                .Include(d => d.Versions.OrderByDescending(v => v.ModifiedAt))
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
            {
                _loggingService.LogSecurityEvent(userId, "DocumentNotFound", $"Attempted to access non-existent document ID: {id}");
                return NotFound();
            }

            _loggingService.LogDocumentOperation(userId, id, "ViewDetails", $"Viewed document: {document.Title}");
            return View(document);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _loggingService.LogDocumentOperation(userId, id, "DeleteDocument", "Attempting to delete document");

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                _loggingService.LogSecurityEvent(userId, "DeleteDocumentNotFound", $"Attempted to delete non-existent document ID: {id}");
                return NotFound();
            }

            // Удаляем физический файл
            if (!string.IsNullOrEmpty(document.FilePath) && System.IO.File.Exists(document.FilePath))
            {
                try
                {
                    System.IO.File.Delete(document.FilePath);
                    _loggingService.LogSystemEvent("FileDelete", $"Physical file deleted: {document.FilePath}");
                }
                catch (Exception ex)
                {
                    _loggingService.LogError(ex, userId, "DeleteFile", $"Error deleting file: {document.FilePath}");
                }
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            _loggingService.LogDocumentOperation(userId, id, "DeleteDocument", $"Document deleted: {document.Title}");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDocument(int id, string title, string status, IFormFile? file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _loggingService.LogDocumentOperation(userId, id, "UpdateDocument", "Starting document update");

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                _loggingService.LogSecurityEvent(userId, "UpdateDocumentNotFound", $"Attempted to update non-existent document ID: {id}");
                return NotFound();
            }

            try
            {
                // Обновляем основные данные
                document.Title = title;
                document.Status = status;

                // Если загружен новый файл
                if (file != null)
                {
                    _loggingService.LogDocumentOperation(userId, id, "FileUpload", 
                        $"New file upload: {file.FileName} ({file.ContentType}, {file.Length} bytes)");

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                        _loggingService.LogSystemEvent("CreateDirectory", $"Created uploads directory: {uploadsFolder}");
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Создаем новую версию
                    var version = new DocumentVersion
                    {
                        DocumentId = document.Id,
                        VersionNumber = (document.Versions.Count + 1).ToString(),
                        FilePath = filePath,
                        ModifiedBy = userId,
                        ModifiedAt = DateTime.UtcNow,
                        ChangeDescription = "Updated document and file"
                    };

                    document.Versions.Add(version);
                    document.FilePath = filePath;
                    document.ContentType = file.ContentType;

                    _loggingService.LogDocumentOperation(userId, id, "NewVersion", 
                        $"Created version {version.VersionNumber}");
                }

                await _context.SaveChangesAsync();
                _loggingService.LogDocumentOperation(userId, id, "UpdateDocument", 
                    $"Document updated successfully: {document.Title}");

                return RedirectToAction(nameof(Details), new { id = document.Id });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, userId, "UpdateDocument", 
                    $"Error updating document ID: {id}");
                throw;
            }
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
                _loggingService.LogSecurityEvent(User.FindFirstValue(ClaimTypes.NameIdentifier), "DownloadNotFound", 
                    $"Attempt to download non-existent or missing file for document ID: {id}");
                return NotFound();
            }

            _loggingService.LogDocumentOperation(User.FindFirstValue(ClaimTypes.NameIdentifier), id, "Download", 
                $"Downloading document: {document.Title}");

            try
            {
                var memory = await System.IO.File.ReadAllBytesAsync(document.FilePath);
                _loggingService.LogDocumentOperation(User.FindFirstValue(ClaimTypes.NameIdentifier), id, "DownloadComplete", 
                    $"Document downloaded ({memory.Length} bytes)");

                // Добавляем заголовки для поддержки Office Online Viewer
                Response.Headers.Append("Access-Control-Allow-Origin", "*");
                Response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
                Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type");

                return File(memory, document.ContentType ?? "application/octet-stream", Path.GetFileName(document.FilePath));
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, User.FindFirstValue(ClaimTypes.NameIdentifier), "Download", 
                    $"Error downloading document ID: {id}");
                throw;
            }
        }

        public async Task<IActionResult> DownloadVersion(int versionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var version = await _context.DocumentVersions.FindAsync(versionId);
            
            if (version == null || string.IsNullOrEmpty(version.FilePath) || !System.IO.File.Exists(version.FilePath))
            {
                _loggingService.LogSecurityEvent(userId, "DownloadVersionNotFound", 
                    $"Attempted to download non-existent or missing version ID: {versionId}");
                return NotFound();
            }

            _loggingService.LogDocumentOperation(userId, version.DocumentId, "DownloadVersion", 
                $"Downloading version {version.VersionNumber}");

            try
            {
                var memory = await System.IO.File.ReadAllBytesAsync(version.FilePath);
                _loggingService.LogDocumentOperation(userId, version.DocumentId, "DownloadComplete", 
                    $"Version {version.VersionNumber} downloaded ({memory.Length} bytes)");

                Response.Headers.Append("Access-Control-Allow-Origin", "*");
                Response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
                Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type");

                return File(memory, "application/octet-stream", Path.GetFileName(version.FilePath));
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, userId, "DownloadVersion", 
                    $"Error downloading version ID: {versionId}");
                throw;
            }
        }

        public async Task<IActionResult> Preview(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _loggingService.LogDocumentOperation(userId, id, "Preview", "Starting document preview");

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                _loggingService.LogSecurityEvent(userId, "PreviewNotFound", 
                    $"Attempted to preview non-existent document ID: {id}");
                return NotFound();
            }

            try
            {
                switch (document.DocumentType.ToLower())
                {
                    case "pdf":
                        if (document.FileContent != null)
                        {
                            _loggingService.LogDocumentOperation(userId, id, "PreviewPDF", 
                                "Serving PDF from database");
                            return File(document.FileContent, "application/pdf");
                        }
                        if (!string.IsNullOrEmpty(document.FilePath) && System.IO.File.Exists(document.FilePath))
                        {
                            _loggingService.LogDocumentOperation(userId, id, "PreviewPDF", 
                                "Serving PDF from file system");
                            return File(await System.IO.File.ReadAllBytesAsync(document.FilePath), "application/pdf");
                        }
                        _loggingService.LogError(new FileNotFoundException(), userId, "PreviewPDF", 
                            $"PDF file not found for document ID: {id}");
                        return NotFound();
                    case "docx":
                        _loggingService.LogDocumentOperation(userId, id, "PreviewDOCX", 
                            "Redirecting to PDF preview for DOCX");
                        return RedirectToAction("PreviewPdf", new { id });
                    default:
                        _loggingService.LogError(new NotSupportedException(), userId, "Preview", 
                            $"Unsupported document type: {document.DocumentType}");
                        return BadRequest($"Preview is not available for {document.DocumentType} documents");
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, userId, "Preview", 
                    $"Error previewing document ID: {id}");
                throw;
            }
        }

        private async Task UpdateDocumentContent(ModelDocument document)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (document.FileContent == null && !string.IsNullOrEmpty(document.FilePath) && System.IO.File.Exists(document.FilePath))
            {
                _loggingService.LogDocumentOperation(userId, document.Id, "UpdateContent", 
                    $"Updating document content from file: {document.FilePath}");

                try
                {
                    document.FileContent = await System.IO.File.ReadAllBytesAsync(document.FilePath);
                    await _context.SaveChangesAsync();
                    _loggingService.LogDocumentOperation(userId, document.Id, "UpdateContent", 
                        $"Content updated successfully, size: {document.FileContent.Length} bytes");
                }
                catch (Exception ex)
                {
                    _loggingService.LogError(ex, userId, "UpdateContent", 
                        $"Error updating document content for ID: {document.Id}");
                    throw;
                }
            }
        }

        // PDF Preview for DOCX using Aspose.Words
        public async Task<IActionResult> PreviewPdf(int id)
        {
            _loggingService.LogDocumentOperation(User.FindFirstValue(ClaimTypes.NameIdentifier), id, "PreviewPdf", "Starting PDF preview");

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                _loggingService.LogSecurityEvent(User.FindFirstValue(ClaimTypes.NameIdentifier), "PreviewPdfNotFound", 
                    $"Document {id} not found");
                return NotFound($"Document {id} not found");
            }

            // Try to update document content if missing
            await UpdateDocumentContent(document);

            _loggingService.LogDocumentOperation(User.FindFirstValue(ClaimTypes.NameIdentifier), id, "PreviewPdf", 
                $"Document found: Id={id}, Type={document.DocumentType}, HasContent={document.FileContent != null}");

            if (document.FileContent == null || document.FileContent.Length == 0)
            {
                _loggingService.LogSecurityEvent(User.FindFirstValue(ClaimTypes.NameIdentifier), "PreviewPdfNoContent", 
                    $"Document {id} has no content");
                return NotFound($"Document {id} has no content");
            }

            try
            {
                if (document.DocumentType.ToLower() == "pdf")
                {
                    _loggingService.LogDocumentOperation(User.FindFirstValue(ClaimTypes.NameIdentifier), id, "PreviewPdfPDF", 
                        $"Serving PDF document directly, size: {document.FileContent.Length} bytes");
                    return File(document.FileContent, "application/pdf");
                }
                else if (document.DocumentType.ToLower() == "docx")
                {
                    _loggingService.LogDocumentOperation(User.FindFirstValue(ClaimTypes.NameIdentifier), id, "PreviewPdfDOCX", 
                        $"Converting DOCX to PDF, input size: {document.FileContent.Length} bytes");
                    using (var inputStream = new MemoryStream(document.FileContent))
                    using (var outputStream = new MemoryStream())
                    {
                        var doc = new AsposeDocument(inputStream);
                        doc.Save(outputStream, Aspose.Words.SaveFormat.Pdf);
                        _loggingService.LogDocumentOperation(User.FindFirstValue(ClaimTypes.NameIdentifier), id, "PreviewPdfDOCXComplete", 
                            $"DOCX successfully converted to PDF, output size: {outputStream.Length} bytes");
                        return File(outputStream.ToArray(), "application/pdf");
                    }
                }
                else
                {
                    _loggingService.LogSecurityEvent(User.FindFirstValue(ClaimTypes.NameIdentifier), "PreviewPdfUnsupported", 
                        $"Unsupported document type: {document.DocumentType}");
                    return BadRequest($"Unsupported file type: {document.DocumentType}");
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, User.FindFirstValue(ClaimTypes.NameIdentifier), "PreviewPdf", 
                    $"Error processing document ID: {id}");
                return StatusCode(500, $"Error processing document: {ex.Message}");
            }
        }

        // Add a method to update all documents
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAllDocumentsContent()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _loggingService.LogUserAction(userId, "UpdateAllDocuments", "Starting bulk document content update");

            var documents = await _context.Documents.ToListAsync();
            var updatedCount = 0;

            foreach (ModelDocument document in documents)
            {
                if (document.FileContent == null && !string.IsNullOrEmpty(document.FilePath) && System.IO.File.Exists(document.FilePath))
                {
                    try
                    {
                        document.FileContent = await System.IO.File.ReadAllBytesAsync(document.FilePath);
                        updatedCount++;
                        _loggingService.LogDocumentOperation(userId, document.Id, "BulkUpdateContent", 
                            $"Updated content for document: {document.Title}");
                    }
                    catch (Exception ex)
                    {
                        _loggingService.LogError(ex, userId, "BulkUpdateContent", 
                            $"Error updating content for document ID: {document.Id}");
                    }
                }
            }

            if (updatedCount > 0)
            {
                await _context.SaveChangesAsync();
                _loggingService.LogUserAction(userId, "UpdateAllDocuments", 
                    $"Completed bulk update, updated {updatedCount} documents");
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 