using Portfolio_AppRepo_API.Models;
using Portfolio_AppRepo_API.Repository.UserRepo;
using Portfolio_AppRepo_API.Services.AuthorizeAD;
using Portfolio_AppRepo_API.ViewModels;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio_AppRepo_API.Classes
{
    sealed class AuthenticationWorker
    {
        private readonly IAuthorizeADService _authorizeAD;
        private readonly IUserRepo _userRepo;
        public AuthenticationWorker(IAuthorizeADService authorizeAD, IUserRepo userRepo)
        {
            _authorizeAD = authorizeAD;
            _userRepo = userRepo;
        }

        internal async Task<User> Authenticate(LoginViewModel UserCredit)
        {
            var AdResult = await AuthenticateUser(UserCredit.Username.Trim(), UserCredit.Password);
            User result = new User();
            if (AdResult)
            {
                var ifExist =  _userRepo.GetAll().Result.FirstOrDefault(x => x.Username.ToLower() == UserCredit.Username.ToLower());
                if (ifExist != null)
                    result = ifExist;
            }
            return result;
        }

        private async Task<bool> AuthenticateUser(string m_UserName, string m_Password)
        {
            //return true;

            var entry = new DirectoryEntry("LDAP://TEST.NET", m_UserName, m_Password);

            //Get connection
            object nativeObject = entry.NativeObject;

            //Search AD
            DirectorySearcher search = new DirectorySearcher(entry);
            search.Filter = "(SAMAccountName=" + m_UserName + ")";
            SearchResult result = search.FindOne();

            if (result != null)
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }
    }
}
