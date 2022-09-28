using Portfolio_AppRepo_API.Models;

namespace Portfolio_AppRepo_API.Repository.AuditRepo
{
    public interface IAuditRepo
    {
        Task<bool> Create(Audit model);
    }
}
