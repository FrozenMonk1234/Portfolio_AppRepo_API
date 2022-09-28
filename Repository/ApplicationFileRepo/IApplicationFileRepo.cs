using Portfolio_AppRepo_API.Models;

namespace Portfolio_AppRepo_API.Repository.ApplicationFileRepo
{
    public interface IApplicationFileRepo
    {
        Task<bool> Create(ApplicationFile model);
        Task<ApplicationFile> GetByApplicationId(int id);
        Task<List<ApplicationFile>> GetAll();
        Task<bool> Update(ApplicationFile model);
        Task<bool> DeleteById(int id);
    }
}
