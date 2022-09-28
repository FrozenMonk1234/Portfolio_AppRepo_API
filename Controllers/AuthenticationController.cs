using Portfolio_AppRepo_API.Classes;
using Portfolio_AppRepo_API.Repository.AuditRepo;
using Portfolio_AppRepo_API.Repository.ErrorLogRepo;
using Portfolio_AppRepo_API.Repository.UserRepo;
using Portfolio_AppRepo_API.Services.AuthorizeAD;
using Portfolio_AppRepo_API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace Portfolio_AppRepo_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly LogWorker logWorker;
        private readonly AuthenticationWorker authWorker;
        public AuthenticationController(IUserRepo userRepo, IAuthorizeADService aDService, IAuditRepo auditRepo, IErrorLogRepo errorLogRepo)
        {
            logWorker = new LogWorker(errorLogRepo, auditRepo);
            authWorker = new AuthenticationWorker(aDService, userRepo);
        }

        [HttpPost]
        [Route("AuthorizeUser")]
        public async Task<IActionResult> AuthorizeUser([FromBody] LoginViewModel model)
        {
            var actionNames = GetControllerActionNames();
            try
            {
                var result = await authWorker.Authenticate(model);
                await logWorker.LogAudit(actionNames, model.Username);
                return Ok(result);
            }
            catch (Exception exc)
            {
                await logWorker.LogError(ControlledAction, $"{exc.Source} - Controller:{ControlledContext}", exc.Message, exc.GetType().ToString());
                throw new Exception($"Something went wrong with:{actionNames}");
            }
        }

        #region Log data handler
        public string ControlledAction { get; set; }
        public string ControlledContext { get; set; }
        public string ControlledUser { get; set; }
        private string GetControllerActionNames()
        {
            ControlledContext = ControllerContext.ActionDescriptor.ControllerName;
            ControlledAction = ControllerContext.ActionDescriptor.ActionName;
            ControlledUser = ControllerContext.HttpContext.User.ToString();
            return $"{ControlledContext} - {ControlledAction}";
        }

        private void GetMethodActionNames()
        {
            StackTrace st = new StackTrace();
            ControlledAction = $"{st.GetFrame(1).GetMethod().Name}";
        }
        #endregion
    }
}
