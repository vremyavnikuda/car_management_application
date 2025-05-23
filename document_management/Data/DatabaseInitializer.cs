using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document_management.Data
{
    public static class DatabaseInitializer
    {
        public static void ApplyMigrations(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                    logger.LogInformation("Миграции успешно применены.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Произошла ошибка при применении миграций.");
                }
            }
        }
    }
} 