using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using document_management.Models;
using document_management.Constants;
using document_management.Data;

namespace document_management.Services
{
    public static class AuthenticationService
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
        {
            // Настройка Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Требования к паролю
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = AuthenticationConstants.PasswordMinLength;
                
                // Требования к пользователю
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Настройка Cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(AuthenticationConstants.CookieExpirationDays);
                options.LoginPath = AuthenticationConstants.LoginPath;
                options.LogoutPath = AuthenticationConstants.LogoutPath;
                options.AccessDeniedPath = AuthenticationConstants.AccessDeniedPath;
                options.SlidingExpiration = true;
            });

            return services;
        }
    }
} 