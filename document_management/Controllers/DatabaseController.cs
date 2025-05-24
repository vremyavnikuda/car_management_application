using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using document_management.Data;
using document_management.Services;
using System.Security.Claims;

namespace document_management.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DatabaseController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggingService _loggingService;
        private readonly IServiceProvider _serviceProvider;

        public DatabaseController(
            IConfiguration configuration,
            ILoggingService loggingService,
            IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _loggingService = loggingService;
            _serviceProvider = serviceProvider;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var backupPath = _configuration["BackupSettings:Path"] ?? "Backups";
            
            _loggingService.LogUserAction(userId, "ViewBackups", "Accessing database backups page");
            
            var backupFiles = DatabaseBackup.GetBackupFilesAsync(backupPath).Result;
            _loggingService.LogDatabaseOperation("ListBackups", $"Found {backupFiles.Count} backup files", true);
            
            return View(backupFiles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBackup()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _loggingService.LogUserAction(userId, "CreateBackup", "Starting database backup creation");

            try
            {
                var backupPath = _configuration["BackupSettings:Path"] ?? "Backups";
                var result = await DatabaseBackup.CreateBackupAsync(_serviceProvider, backupPath);
                
                if (result)
                {
                    _loggingService.LogDatabaseOperation("CreateBackup", "Database backup created successfully", true);
                    TempData["SuccessMessage"] = "Резервная копия успешно создана";
                }
                else
                {
                    _loggingService.LogDatabaseOperation("CreateBackup", "Failed to create database backup", false);
                    TempData["ErrorMessage"] = "Ошибка при создании резервной копии";
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, userId, "CreateBackup", "Error creating database backup");
                TempData["ErrorMessage"] = "Произошла ошибка при создании резервной копии";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RestoreBackup(string backupFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _loggingService.LogUserAction(userId, "RestoreBackup", $"Starting database restore from: {backupFile}");

            try
            {
                var result = await DatabaseBackup.RestoreBackupAsync(_serviceProvider, backupFile);
                
                if (result)
                {
                    _loggingService.LogDatabaseOperation("RestoreBackup", $"Database restored successfully from: {backupFile}", true);
                    TempData["SuccessMessage"] = "База данных успешно восстановлена";
                }
                else
                {
                    _loggingService.LogDatabaseOperation("RestoreBackup", $"Failed to restore database from: {backupFile}", false);
                    TempData["ErrorMessage"] = "Ошибка при восстановлении базы данных";
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, userId, "RestoreBackup", $"Error restoring database from: {backupFile}");
                TempData["ErrorMessage"] = "Произошла ошибка при восстановлении базы данных";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult DownloadBackup(string backupFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _loggingService.LogUserAction(userId, "DownloadBackup", $"Attempting to download backup file: {backupFile}");

            try
            {
                if (!System.IO.File.Exists(backupFile))
                {
                    _loggingService.LogSecurityEvent(userId, "BackupFileNotFound", $"Backup file not found: {backupFile}");
                    return NotFound();
                }

                var fileName = Path.GetFileName(backupFile);
                var fileBytes = System.IO.File.ReadAllBytes(backupFile);
                
                _loggingService.LogUserAction(userId, "DownloadBackup", $"Successfully downloaded backup file: {fileName}");
                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, userId, "DownloadBackup", $"Error downloading backup file: {backupFile}");
                TempData["ErrorMessage"] = "Произошла ошибка при скачивании резервной копии";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 