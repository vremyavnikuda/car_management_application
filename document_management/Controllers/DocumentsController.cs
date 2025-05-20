using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using document_management.Data;
using document_management.Models;
using System.Security.Claims;
using document_management.Models.ViewModels;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using System.Web;

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

                // Добавляем заголовки для поддержки Office Online Viewer
                Response.Headers.Add("Access-Control-Allow-Origin", "*");
                Response.Headers.Add("Access-Control-Allow-Methods", "GET, OPTIONS");
                Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

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

                // Добавляем заголовки для поддержки Office Online Viewer
                Response.Headers.Add("Access-Control-Allow-Origin", "*");
                Response.Headers.Add("Access-Control-Allow-Methods", "GET, OPTIONS");
                Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                return File(memory, "application/octet-stream", Path.GetFileName(version.FilePath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading version {VersionId}", version.Id);
                throw;
            }
        }

        private string ConvertDocxToHtml(string filePath)
        {
            try
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, false))
                {
                    var body = doc.MainDocumentPart?.Document.Body;
                    if (body == null) return string.Empty;

                    var html = new StringBuilder();
                    html.AppendLine("<!DOCTYPE html>");
                    html.AppendLine("<html>");
                    html.AppendLine("<head>");
                    html.AppendLine("<meta charset='utf-8'>");
                    html.AppendLine("<style>");
                    html.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; padding: 20px; }");
                    html.AppendLine("p { margin-bottom: 1em; }");
                    html.AppendLine("h1, h2, h3, h4, h5, h6 { margin-top: 1.5em; margin-bottom: 0.5em; }");
                    html.AppendLine("</style>");
                    html.AppendLine("</head>");
                    html.AppendLine("<body>");

                    foreach (var element in body.Elements())
                    {
                        if (element is Paragraph paragraph)
                        {
                            var paragraphText = new StringBuilder();
                            foreach (var run in paragraph.Elements<Run>())
                            {
                                var text = run.GetFirstChild<Text>();
                                if (text != null)
                                {
                                    paragraphText.Append(text.Text);
                                }
                            }
                            if (paragraphText.Length > 0)
                            {
                                html.AppendLine($"<p>{HttpUtility.HtmlEncode(paragraphText.ToString())}</p>");
                            }
                        }
                    }

                    html.AppendLine("</body>");
                    html.AppendLine("</html>");
                    return html.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting DOCX to HTML for file {FilePath}", filePath);
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

            _logger.LogInformation("User {User} previewing document {DocumentId} ({Title})", 
                User.Identity?.Name, document.Id, document.Title);

            try
            {
                switch (document.DocumentType.ToLower())
                {
                    case "pdf":
                        var pdfMemory = await System.IO.File.ReadAllBytesAsync(document.FilePath);
                        _logger.LogInformation("Document {DocumentId} successfully loaded for PDF preview ({FileSize} bytes)", 
                            document.Id, pdfMemory.Length);
                        return File(pdfMemory, "application/pdf");

                    case "docx":
                        // Генерируем публичный URL для документа
                        var fileUrl = Url.Action("Download", "Documents", new { id = document.Id }, Request.Scheme);
                        
                        // Формируем URL для Office Online Viewer
                        var officeViewerUrl = $"https://view.officeapps.live.com/op/embed.aspx?src={Uri.EscapeDataString(fileUrl)}";
                        
                        _logger.LogInformation("Document {DocumentId} redirected to Office Online Viewer", document.Id);
                        
                        // Создаем HTML страницу с iframe для Office Online Viewer
                        var html = $@"
                            <!DOCTYPE html>
                            <html>
                            <head>
                                <meta charset='utf-8'>
                                <title>{HttpUtility.HtmlEncode(document.Title)}</title>
                                <style>
                                    body, html {{
                                        margin: 0;
                                        padding: 0;
                                        height: 100%;
                                        overflow: hidden;
                                    }}
                                    iframe {{
                                        width: 100%;
                                        height: 100%;
                                        border: none;
                                    }}
                                </style>
                            </head>
                            <body>
                                <iframe src='{officeViewerUrl}' frameborder='0' allowfullscreen='true'></iframe>
                            </body>
                            </html>";

                        return Content(html, "text/html");

                    default:
                        _logger.LogWarning("Attempt to preview unsupported document type {DocumentType} for document {DocumentId} by user {User}", 
                            document.DocumentType, id, User.Identity?.Name);
                        return BadRequest($"Preview is not available for {document.DocumentType} documents");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing document {DocumentId}", document.Id);
                throw;
            }
        }
    }
} 