using Portfolio_AppRepo_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Portfolio_AppRepo_API.Repository.UserRepo
{
    public class UserRepo : IUserRepo
    {
        ApplicationRepoContext _db;
        public UserRepo(ApplicationRepoContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(User model)
        {
            await _db.Users.AddAsync(model);
            if (await _db.SaveChangesAsync() > 0)

                return true;
            else
                return false;
        }

        public Task<bool> DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<User>> GetAll()
        {
            return await _db.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> Update(User model)
        {
            _db.Users.Update(model);
            if (await _db.SaveChangesAsync() > 0)
                return true;
            else
                return false;
        }
    }
}
