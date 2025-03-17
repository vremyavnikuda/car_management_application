// TODO: Логирование событий
namespace CarManagementApp
{
    public static class Logger
    {
        private static readonly string logFilePath = "app.log";

        public static void Log(string message)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                {
                    sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                }
            }
            catch
            {
                // Если логирование не удалось, ничего не делаем
            }
        }

        public static void LogException(Exception ex)
        {
            Log($"ERROR: {ex.Message}\n{ex.StackTrace}");
        }
    }
}