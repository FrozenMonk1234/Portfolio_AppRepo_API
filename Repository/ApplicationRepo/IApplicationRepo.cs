using Portfolio_AppRepo_API.Models;

namespace Portfolio_AppRepo_API.Repository.ApplicationRepo
{
    public interface IApplicationRepo
    {
        Task<int> Create(Application model);
        Task<Application> GetById(int id);
        Task<List<Application>> GetAll();
        Task<bool> Update (Application model);
        Task<bool>  DeleteById(int id);
    }
}
