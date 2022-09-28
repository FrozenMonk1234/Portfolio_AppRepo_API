using Portfolio_AppRepo_API.Models;
using Endpoint = Portfolio_AppRepo_API.Models.Endpoint;

namespace Portfolio_AppRepo_API.Repository.EndpointRepo
{
    public interface IEndpointRepo
    {
        Task<bool> Create(Endpoint model);
        Task<bool> CreateRange(List<Models.Endpoint> list);
        Task<bool> UpdateRange(List<Models.Endpoint> list);
        Task<Endpoint> GetById(int id);
        Task<List<Endpoint>> GetAll();
        Task<List<Endpoint>> GetAllByApplicationId(int applicationId);
        Task<bool> Update(Endpoint model);
        Task<bool> DeleteById(int id);
        Task<bool> DeleteRangeByApplictionId(int Id);

    }
}
