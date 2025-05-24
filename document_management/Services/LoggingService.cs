using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace document_management.Services
{
    public interface ILoggingService
    {
        void LogUserAction(string userId, string action, string details, [CallerMemberName] string methodName = "");
        void LogDocumentOperation(string userId, int documentId, string operation, string details, [CallerMemberName] string methodName = "");
        void LogSecurityEvent(string userId, string eventType, string details, [CallerMemberName] string methodName = "");
        void LogDatabaseOperation(string operation, string details, bool success, [CallerMemberName] string methodName = "");
        void LogSystemEvent(string eventType, string details, [CallerMemberName] string methodName = "");
        void LogError(Exception ex, string userId, string operation, string details, [CallerMemberName] string methodName = "");
    }

    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public void LogUserAction(string userId, string action, string details, [CallerMemberName] string methodName = "")
        {
            _logger.LogInformation(
                "User Action: {Action} | User: {UserId} | Method: {Method} | Details: {Details}",
                action, userId, methodName, details);
        }

        public void LogDocumentOperation(string userId, int documentId, string operation, string details, [CallerMemberName] string methodName = "")
        {
            _logger.LogInformation(
                "Document Operation: {Operation} | Document: {DocumentId} | User: {UserId} | Method: {Method} | Details: {Details}",
                operation, documentId, userId, methodName, details);
        }

        public void LogSecurityEvent(string userId, string eventType, string details, [CallerMemberName] string methodName = "")
        {
            _logger.LogWarning(
                "Security Event: {EventType} | User: {UserId} | Method: {Method} | Details: {Details}",
                eventType, userId, methodName, details);
        }

        public void LogDatabaseOperation(string operation, string details, bool success, [CallerMemberName] string methodName = "")
        {
            if (success)
            {
                _logger.LogInformation(
                    "Database Operation: {Operation} | Method: {Method} | Details: {Details}",
                    operation, methodName, details);
            }
            else
            {
                _logger.LogError(
                    "Database Operation Failed: {Operation} | Method: {Method} | Details: {Details}",
                    operation, methodName, details);
            }
        }

        public void LogSystemEvent(string eventType, string details, [CallerMemberName] string methodName = "")
        {
            _logger.LogInformation(
                "System Event: {EventType} | Method: {Method} | Details: {Details}",
                eventType, methodName, details);
        }

        public void LogError(Exception ex, string userId, string operation, string details, [CallerMemberName] string methodName = "")
        {
            _logger.LogError(ex,
                "Error: {Operation} | User: {UserId} | Method: {Method} | Details: {Details} | Exception: {ExceptionMessage}",
                operation, userId, methodName, details, ex.Message);
        }
    }
} 