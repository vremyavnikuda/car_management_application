namespace document_management.Constants
{
    public static class AuthenticationConstants
    {
        public const string LoginPath = "/Account/Login";
        public const string LogoutPath = "/Account/Logout";
        public const string AccessDeniedPath = "/Account/AccessDenied";
        
        public const int CookieExpirationDays = 7;
        public const int PasswordMinLength = 8;
    }
} 