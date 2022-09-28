using Portfolio_AppRepo_API.Models;

namespace Portfolio_AppRepo_API.Repository.AuditRepo
{
    public class AuditRepo : IAuditRepo
    {
        ApplicationRepoContext _db;
        public AuditRepo(ApplicationRepoContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(Audit model)
        {
            await _db.Audits.AddAsync(model);
            if (await _db.SaveChangesAsync() > 0)

                return true;
            else
                return false;
        }
    }
}
