using Portfolio_AppRepo_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Portfolio_AppRepo_API.Repository.EndpointRepo
{
    public class EndpointRepo : IEndpointRepo
    {
        ApplicationRepoContext _db;
        public EndpointRepo(ApplicationRepoContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(Models.Endpoint model)
        {
            await _db.Endpoints.AddAsync(model);
            if (await _db.SaveChangesAsync() > 0)

                return true;
            else
                return false;
        }

        public async Task<bool> CreateRange(List<Models.Endpoint> list)
        {
            await _db.Endpoints.AddRangeAsync(list);
            if (await _db.SaveChangesAsync() > 0)

                return true;
            else
                return false;
        }

        public async Task<bool> DeleteById(int id)
        {
            var item = await _db.Endpoints.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                _db.Endpoints.Remove(item);
            }
            var result = await _db.SaveChangesAsync();
            if (result > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> DeleteRangeByApplictionId(int Id)
        {
            var deleteList = await _db.Endpoints.Where(x => x.ApplicationId == Id).ToListAsync();
            _db.RemoveRange(deleteList);
            var result = await _db.SaveChangesAsync();
            if(result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<Models.Endpoint>> GetAll()
        {
            return await _db.Endpoints.Where(x => x.IsDelete == false).ToListAsync();
        }

        public async Task<List<Models.Endpoint>> GetAllByApplicationId(int applicationId)
        {

            return await _db.Endpoints.AsNoTracking().Where(x => x.ApplicationId == applicationId && x.IsDelete == false).ToListAsync();
        }

        public async Task<Models.Endpoint> GetById(int id)
        {
            return await _db.Endpoints.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == false);
        }

        public async Task<bool> Update(Models.Endpoint model)
        {
            _db.Update(model);
            if (await _db.SaveChangesAsync() > 0)
            {
                return true;
            }
            else
                return false;
        }

        public async Task<bool> UpdateRange(List<Models.Endpoint> list)
        {
            _db.Endpoints.UpdateRange(list);
            if (await _db.SaveChangesAsync() > 0)
                return true;
            else
                return false;
        }
    }
}
