using Portfolio_AppRepo_API.Classes;
using Portfolio_AppRepo_API.Models;
using Portfolio_AppRepo_API.Repository.AuditRepo;
using Portfolio_AppRepo_API.Repository.ErrorLogRepo;
using Portfolio_AppRepo_API.Repository.UserRepo;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Portfolio_AppRepo_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepo _userRepo;
        private readonly LogWorker logWorker;
        public UserController(IUserRepo userRepo, IAuditRepo auditRepo, IErrorLogRepo errorLogRepo)
        {
            _userRepo = userRepo;
            logWorker = new LogWorker(errorLogRepo, auditRepo);
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var actionNames = GetControllerActionNames();
            try
            {
                var result = await _userRepo.GetAll();
                return Ok(result);
            }
            catch (Exception exc)
            {
                await logWorker.LogError(ControlledAction, $"{exc.Source} - Controller:{ControlledContext}", exc.Message, exc.GetType().ToString());
                throw new Exception($"Something went wrong with:{actionNames}");
            }
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {
            var actionNames = GetControllerActionNames();
            try
            {
                var result = await _userRepo.GetById(Id);
                return Ok(result);
            }
            catch (Exception exc)
            {
                logWorker.LogError(ControlledAction, $"{exc.Source} - Controller:{ControlledContext}", exc.Message, exc.GetType().ToString());
                throw new Exception($"Something went wrong with:{actionNames}");
            }
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] User model)
        {
            var actionNames = GetControllerActionNames();
            try
            {
                var result = await _userRepo.Update(model);
                await logWorker.LogAudit(actionNames, model.Username);
                return Ok(result);
            }
            catch (Exception exc)
            {
                await logWorker.LogError(ControlledAction, $"{exc.Source} - Controller:{ControlledContext}", exc.Message, exc.GetType().ToString());
                throw new Exception($"Something went wrong with:{actionNames}");
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] User model)
        {
            var actionNames = GetControllerActionNames();
            try
            {
                var result = await _userRepo.Create(model);
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
