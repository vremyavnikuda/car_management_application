using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using document_management.Data;
using document_management.Models;
using document_management.Models.ViewModels;
using document_management.Services;
using System.Security.Claims;
using System.IO;

namespace document_management.Controllers
{
    [Authorize]
    public class DocumentUploadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoggingService _loggingService;

        public DocumentUploadController(
            ApplicationDbContext context,
            ILoggingService loggingService)
        {
            _context = context;
            _loggingService = loggingService;
        }

        // GET: DocumentUpload/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _loggingService.LogUserAction(userId, "AccessUploadForm", "User accessed document upload form");
            
            var viewModel = new DocumentUploadViewModel
            {
                Title = string.Empty,
                DocumentType = string.Empty,
                Status = string.Empty
            };
            return View(viewModel);
        }

        // POST: DocumentUpload/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentUploadViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                _loggingService.LogUserAction(userId, "InvalidUploadAttempt", "Invalid document upload attempt");
                return View(model);
            }

            try
            {
                _loggingService.LogUserAction(userId, "UploadAttempt", 
                    $"Attempting to upload document: {model.Title}");

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    _loggingService.LogSystemEvent("CreateDirectory", $"Created uploads directory: {uploadsFolder}");
                }

                if (model.File == null || model.File.Length == 0)
                {
                    _loggingService.LogUserAction(userId, "EmptyFileUpload", "User attempted to upload empty file");
                    ModelState.AddModelError("File", "Пожалуйста, выберите файл для загрузки");
                    return View(model);
                }

                _loggingService.LogUserAction(userId, "ProcessingUpload", 
                    $"Processing file upload: {model.File.FileName} ({model.File.ContentType}, {model.File.Length} bytes)");

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.File.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                _loggingService.LogSystemEvent("FileSaved", $"File saved to disk: {filePath}");

                // Save file content to database
                byte[] fileContent;
                using (var memoryStream = new MemoryStream())
                {
                    await model.File.CopyToAsync(memoryStream);
                    fileContent = memoryStream.ToArray();
                }

                _loggingService.LogSystemEvent("ContentProcessed", 
                    $"File content processed in memory, size: {fileContent.Length} bytes");

                var document = new Document
                {
                    Title = model.Title,
                    DocumentType = model.DocumentType,
                    Status = model.Status,
                    FilePath = filePath,
                    ContentType = model.File.ContentType,
                    FileContent = fileContent,
                    Author = User.FindFirstValue(ClaimTypes.Email) ?? "Unknown",
                    CreatedAt = DateTime.UtcNow
                };

                // Create first version
                var version = new DocumentVersion
                {
                    VersionNumber = "1",
                    FilePath = filePath,
                    ModifiedBy = User.FindFirstValue(ClaimTypes.Email) ?? "Unknown",
                    ModifiedAt = DateTime.UtcNow,
                    ChangeDescription = "Initial version"
                };

                document.Versions.Add(version);
                _context.Documents.Add(document);
                await _context.SaveChangesAsync();

                _loggingService.LogDocumentOperation(userId, document.Id, "DocumentCreated", 
                    $"Document created: {document.Title} (Type: {document.DocumentType}, Size: {document.FileContent?.Length ?? 0} bytes)");

                // Добавляем сообщение об успешной загрузке
                TempData["SuccessMessage"] = $"Документ '{model.Title}' успешно загружен.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, userId, "UploadError", 
                    $"Error uploading document: {model.Title}");
                ModelState.AddModelError("", "Произошла ошибка при загрузке документа. Пожалуйста, попробуйте снова.");
                return View(model);
            }
        }
    }
} 