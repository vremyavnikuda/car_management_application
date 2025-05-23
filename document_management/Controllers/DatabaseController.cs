using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using document_management.Data;
using System.Diagnostics;

namespace document_management.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DatabaseController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseController> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DatabaseController(
            IConfiguration configuration,
            ILogger<DatabaseController> logger,
            IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public IActionResult Index()
        {
            var backupPath = _configuration["BackupSettings:Path"] ?? "Backups";
            var backupFiles = DatabaseBackup.GetBackupFilesAsync(backupPath).Result;
            return View(backupFiles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBackup()
        {
            try
            {
                var backupPath = _configuration["BackupSettings:Path"] ?? "Backups";
                var result = await DatabaseBackup.CreateBackupAsync(_serviceProvider, backupPath);
                
                if (result)
                {
                    TempData["SuccessMessage"] = "Резервная копия успешно создана";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ошибка при создании резервной копии";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании резервной копии");
                TempData["ErrorMessage"] = "Произошла ошибка при создании резервной копии";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RestoreBackup(string backupFile)
        {
            try
            {
                var result = await DatabaseBackup.RestoreBackupAsync(_serviceProvider, backupFile);
                
                if (result)
                {
                    TempData["SuccessMessage"] = "База данных успешно восстановлена";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ошибка при восстановлении базы данных";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при восстановлении базы данных");
                TempData["ErrorMessage"] = "Произошла ошибка при восстановлении базы данных";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult DownloadBackup(string backupFile)
        {
            try
            {
                if (!System.IO.File.Exists(backupFile))
                {
                    return NotFound();
                }

                var fileName = Path.GetFileName(backupFile);
                var fileBytes = System.IO.File.ReadAllBytes(backupFile);
                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при скачивании резервной копии");
                TempData["ErrorMessage"] = "Произошла ошибка при скачивании резервной копии";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 