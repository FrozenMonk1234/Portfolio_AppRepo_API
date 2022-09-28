using Portfolio_AppRepo_API.Models;
using Portfolio_AppRepo_API.Repository.AuditRepo;
using Portfolio_AppRepo_API.Repository.ErrorLogRepo;

namespace Portfolio_AppRepo_API.Classes
{
    public class LogWorker
    {
        private readonly IErrorLogRepo _errorLog;
        private readonly IAuditRepo _auditRepo;
        public LogWorker(IErrorLogRepo errorLogRepo, IAuditRepo auditRepo)
        {
            _auditRepo = auditRepo;
            _errorLog = errorLogRepo;
        }

        public async Task LogError(string innerException, string exceptionSource, string exceptionMessage, string exceptionType)
        {
            ErrorLog log = new ErrorLog();
            log.InnerException = $"{innerException}";
            log.ExceptionSource = $"{exceptionSource}";
            log.ExceptionMessage = $"{exceptionMessage}";
            log.Date = DateTime.Now;
            log.ExceptionType = $"{exceptionType}";

            await _errorLog.Create(log);
        }

        public async Task LogAudit(string action, string user)
        {
            Audit audit = new Audit();
            audit.Action = $"{action}";
            audit.User = $"{user}";
            audit.Date = DateTime.Now;

            await _auditRepo.Create(audit);
        }
    }
}
