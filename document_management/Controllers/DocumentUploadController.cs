using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using document_management.Data;
using document_management.Models;
using document_management.Models.ViewModels;
using System.Security.Claims;

namespace document_management.Controllers
{
    [Authorize]
    public class DocumentUploadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DocumentUploadController> _logger;

        public DocumentUploadController(ApplicationDbContext context, ILogger<DocumentUploadController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: DocumentUpload/Create
        public IActionResult Create()
        {
            _logger.LogInformation("User {User} accessed document upload form", User.Identity?.Name);
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
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid document upload attempt by user {User}", User.Identity?.Name);
                return View(model);
            }

            try
            {
                _logger.LogInformation("User {User} attempting to upload document: {Title}", 
                    User.Identity?.Name, model.Title);

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    _logger.LogInformation("Created uploads directory: {UploadsFolder}", uploadsFolder);
                }

                if (model.File == null || model.File.Length == 0)
                {
                    ModelState.AddModelError("File", "Пожалуйста, выберите файл для загрузки");
                    return View(model);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.File.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                _logger.LogInformation("File successfully saved to {FilePath}", filePath);

                var document = new Document
                {
                    Title = model.Title,
                    DocumentType = model.DocumentType,
                    Status = model.Status,
                    FilePath = filePath,
                    ContentType = model.File.ContentType,
                    Author = User.FindFirstValue(ClaimTypes.Email) ?? "Unknown",
                    CreatedAt = DateTime.UtcNow
                };

                // Создаем первую версию документа
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

                _logger.LogInformation("Document {DocumentId} successfully created by user {User}", 
                    document.Id, User.Identity?.Name);

                // Добавляем сообщение об успешной загрузке
                TempData["SuccessMessage"] = $"Документ '{model.Title}' успешно загружен.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document for user {User}", User.Identity?.Name);
                ModelState.AddModelError("", "Произошла ошибка при загрузке документа. Пожалуйста, попробуйте снова.");
                return View(model);
            }
        }
    }
} 