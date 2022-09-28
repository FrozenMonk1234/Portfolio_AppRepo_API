using Portfolio_AppRepo_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Portfolio_AppRepo_API.Repository.ApplicationFileRepo
{
    public class ApplicationFileRepo : IApplicationFileRepo
    {
        ApplicationRepoContext _db;
        public ApplicationFileRepo(ApplicationRepoContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(ApplicationFile model)
        {
            await _db.ApplicationFiles.AddAsync(model);
            if (await _db.SaveChangesAsync() > 0)

                return true;
            else
                return false;
        }

        public Task<bool> DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationFile>> GetAll()
        {
            return await _db.ApplicationFiles.ToListAsync();
        }

        public async Task<ApplicationFile> GetByApplicationId(int id)
        {
            return await _db.ApplicationFiles.FirstOrDefaultAsync(x => x.ApplicationId == id);
        }

        public async Task<bool> Update(ApplicationFile model)
        {
            _db.ApplicationFiles.Update(model);
            if (await _db.SaveChangesAsync() > 0)
                return true;
            else
                return false;
        }
    }
}
