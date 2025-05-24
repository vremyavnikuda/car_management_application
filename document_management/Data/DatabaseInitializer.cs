using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using document_management.Services;

namespace document_management.Data
{
    public static class DatabaseInitializer
    {
        public static void ApplyMigrations(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggingService = services.GetRequiredService<ILoggingService>();

                try
                {
                    loggingService.LogDatabaseOperation(
                        "InitializeDatabase",
                        "Starting database initialization and migrations",
                        true);

                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();

                    loggingService.LogDatabaseOperation(
                        "InitializeDatabase",
                        "Database migrations applied successfully",
                        true);

                    // Проверяем и создаем роли по умолчанию
                    EnsureRoles(services, loggingService);
                }
                catch (Exception ex)
                {
                    loggingService.LogError(
                        ex,
                        "system",
                        "DatabaseInitialization",
                        "Error during database initialization");
                    throw;
                }
            }
        }

        private static void EnsureRoles(IServiceProvider services, ILoggingService loggingService)
        {
            try
            {
                var roleManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();

                string[] roles = new string[] { "Admin", "User" };

                foreach (string role in roles)
                {
                    if (!roleManager.RoleExistsAsync(role).Result)
                    {
                        var result = roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(role)).Result;
                        if (result.Succeeded)
                        {
                            loggingService.LogSystemEvent(
                                "RoleCreation",
                                $"Created role: {role}");
                        }
                        else
                        {
                            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                            loggingService.LogError(
                                new Exception($"Role creation failed: {errors}"),
                                "system",
                                "RoleCreation",
                                $"Failed to create role: {role}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                loggingService.LogError(
                    ex,
                    "system",
                    "RoleInitialization",
                    "Error during role initialization");
                throw;
            }
        }
    }
} 