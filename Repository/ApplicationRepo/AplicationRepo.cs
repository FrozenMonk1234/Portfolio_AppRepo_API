using Portfolio_AppRepo_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Portfolio_AppRepo_API.Repository.ApplicationRepo
{
    public class AplicationRepo : IApplicationRepo
    {
        ApplicationRepoContext _db;
        public AplicationRepo(ApplicationRepoContext db)
        {
            _db = db;
        }

        public async Task<int> Create(Application model)
        {
           
            await _db.Applications.AddAsync(model);
            await _db.SaveChangesAsync();
            return model.Id;
        }
        public async Task<bool> DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Application>> GetAll()
        {
            // return await _db.Applications.Where(x => x.IsDelete.Equals(false)).ToListAsync();
            return await _db.Applications.Where(x => x.IsDelete == false).ToListAsync();
        }

        public async Task<Application> GetById(int id)
        {
            return await _db.Applications.FirstOrDefaultAsync(x => x.Id == id && x.IsDelete.Equals(false));
        }

        public async Task<bool> Update(Application model)
        {
            _db.Applications.Update(model);
            if (await _db.SaveChangesAsync() > 0)
                return true;
            else
                return false;
        }
    }
}
