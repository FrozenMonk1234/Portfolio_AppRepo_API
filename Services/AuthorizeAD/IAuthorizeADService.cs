using Portfolio_AppRepo_API.Models;

namespace Portfolio_AppRepo_API.Services.AuthorizeAD
{
    public interface IAuthorizeADService
    {
        Task<User> Login (string username, string password);
    }
}
