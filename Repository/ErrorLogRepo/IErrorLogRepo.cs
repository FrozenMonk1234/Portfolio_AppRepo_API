using Portfolio_AppRepo_API.Models;

namespace Portfolio_AppRepo_API.Repository.ErrorLogRepo
{
    public interface IErrorLogRepo
    {
        Task<bool> Create(ErrorLog model);
    }
}
