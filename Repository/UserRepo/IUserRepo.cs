        using Portfolio_AppRepo_API.Models;

namespace Portfolio_AppRepo_API.Repository.UserRepo
{
    public interface IUserRepo
    {
        Task<bool> Create(User model);
        Task<User> GetById(int id);
        Task<List<User>> GetAll();
        Task<bool> Update(User model);
        Task<bool> DeleteById(int id);
    }
}
