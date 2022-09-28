using Portfolio_AppRepo_API.Models;

namespace Portfolio_AppRepo_API.Repository.ErrorLogRepo
{
    public class ErrorLogRepo : IErrorLogRepo
    {
        ApplicationRepoContext _db;
        public ErrorLogRepo(ApplicationRepoContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(ErrorLog model)
        {
            await _db.ErrorLogs.AddAsync(model);
            if (await _db.SaveChangesAsync() > 0)

                return true;
            else
                return false;
        }
    }
}
