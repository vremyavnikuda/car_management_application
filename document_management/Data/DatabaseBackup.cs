using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace document_management.Data
{
    public static class DatabaseBackup
    {
        public static async Task<bool> CreateBackupAsync(IServiceProvider serviceProvider, string backupPath)
        {
            try
            {
                var services = serviceProvider.CreateScope().ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<Program>>();
                var context = services.GetRequiredService<ApplicationDbContext>();

                // Получаем строку подключения
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                var connection = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);

                // Формируем команду для pg_dump
                var dumpFile = Path.Combine(backupPath, $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql");
                var pgDumpArgs = $"-h {connection.Host} -p {connection.Port} -U {connection.Username} -F c -b -v -f \"{dumpFile}\" {connection.Database}";

                // Создаем процесс для выполнения pg_dump
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "pg_dump",
                        Arguments = pgDumpArgs,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                // Устанавливаем переменную окружения для пароля
                process.StartInfo.EnvironmentVariables["PGPASSWORD"] = connection.Password;

                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    logger.LogInformation($"Резервная копия успешно создана: {dumpFile}");
                    return true;
                }
                else
                {
                    logger.LogError($"Ошибка при создании резервной копии: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Произошла ошибка при создании резервной копии");
                return false;
            }
        }

        public static async Task<bool> RestoreBackupAsync(IServiceProvider serviceProvider, string backupFile)
        {
            try
            {
                var services = serviceProvider.CreateScope().ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<Program>>();
                var context = services.GetRequiredService<ApplicationDbContext>();

                // Получаем строку подключения
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                var connection = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);

                // Формируем команду для pg_restore
                var pgRestoreArgs = $"-h {connection.Host} -p {connection.Port} -U {connection.Username} -d {connection.Database} -v \"{backupFile}\"";

                // Создаем процесс для выполнения pg_restore
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "pg_restore",
                        Arguments = pgRestoreArgs,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                // Устанавливаем переменную окружения для пароля
                process.StartInfo.EnvironmentVariables["PGPASSWORD"] = connection.Password;

                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    logger.LogInformation($"База данных успешно восстановлена из файла: {backupFile}");
                    return true;
                }
                else
                {
                    logger.LogError($"Ошибка при восстановлении базы данных: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Произошла ошибка при восстановлении базы данных");
                return false;
            }
        }

        public static async Task<List<string>> GetBackupFilesAsync(string backupPath)
        {
            try
            {
                if (!Directory.Exists(backupPath))
                {
                    Directory.CreateDirectory(backupPath);
                }

                var files = await Task.Run(() => Directory.GetFiles(backupPath, "backup_*.sql")
                    .OrderByDescending(f => File.GetLastWriteTime(f))
                    .ToList());

                return files;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении списка резервных копий", ex);
            }
        }
    }
} 