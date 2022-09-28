using Portfolio_AppRepo_API.Classes;
using Portfolio_AppRepo_API.Models;
using Portfolio_AppRepo_API.Repository.ApplicationFileRepo;
using Portfolio_AppRepo_API.Repository.ApplicationRepo;
using Portfolio_AppRepo_API.Repository.AuditRepo;
using Portfolio_AppRepo_API.Repository.EndpointRepo;
using Portfolio_AppRepo_API.Repository.ErrorLogRepo;
using Portfolio_AppRepo_API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Portfolio_AppRepo_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : Controller
    {
        private readonly LogWorker logWorker;
        private readonly ApplicationWorker applicationWorker;
        public ApplicationController(
            IApplicationRepo applicationRepo,
            IApplicationFileRepo applicationFileRepo,
            IEndpointRepo endpointRepo,
            IAuditRepo auditRepo,
            IErrorLogRepo errorLogRepo)
        {
            logWorker = new LogWorker(errorLogRepo, auditRepo);
            applicationWorker = new ApplicationWorker(applicationRepo, endpointRepo, applicationFileRepo);
        }

        [HttpPost]
        [Route("CreateNewEntry")]
        public async Task<IActionResult> CreateNewEntry([FromBody] ApplicationViewModel model)
        {
            var actionNames = GetControllerActionNames();
            try
            {
                var result = await applicationWorker.CreateAppliactionAsync(model);
                await logWorker.LogAudit(actionNames, model.User);
                return Ok(result);
            }
            catch (Exception exc)
            {
                await logWorker.LogError(ControlledAction, $"{exc.Source} - Controller:{ControlledContext}", exc.Message, exc.GetType().ToString());
                throw new Exception($"Something went wrong with:{actionNames}");
            }
        }

        [HttpGet]
        [Route("GetEntryById")]
        public async Task<IActionResult> GetEntryById(int Id)
        {
            var actionNames = GetControllerActionNames();
            try
            {
                var result = await applicationWorker.GetApplicationById(Id);
                return Ok(result);
            }
            catch (Exception exc)
            {
                await logWorker.LogError(ControlledAction, $"{exc.Source} - Controller:{ControlledContext}", exc.Message, exc.GetType().ToString());
                throw new Exception($"Something went wrong with: {actionNames}");
            }
        }

        [HttpGet]
        [Route("GetAllEntries")]
        public async Task<IActionResult> GetAllEntries()
        {
            var actionNames = GetControllerActionNames();
            try
            {
                var result = await applicationWorker.GetAllApplications();
                return Ok(result);
            }
            catch (Exception exc)
            {
                await logWorker.LogError(ControlledAction, $"{exc.Source} - Controller:{ControlledContext}", exc.Message, exc.GetType().ToString());
                throw new Exception($"Something went wrong with: {actionNames}");
            }
        }

        [HttpPost]
        [Route("UpdateEntry")]
        public async Task<IActionResult> UpdateEntry(ApplicationViewModel model)
        {
            var actionNames = GetControllerActionNames();
            try
            {
                var result = await applicationWorker.UpdateApplicationAsync(model);
                await logWorker.LogAudit(actionNames, model.User);
                return Ok(result);
            }
            catch (Exception exc)
            {
                await logWorker.LogError(ControlledAction, $"{exc.Source} - Controller:{ControlledContext}", exc.Message, exc.GetType().ToString());
                throw new Exception($"Something went wrong with:{actionNames}");
            }
        }

        [HttpGet]
        [Route("DeleteEntryById")]
        public async Task<IActionResult> DeleteEntryById(int Id, string User)
        {
            var actionNames = GetControllerActionNames();
            try
            {
                var result = await applicationWorker.DeleteApplicationById(Id, User);

                await logWorker.LogAudit(actionNames, User);
                return Ok(result);
            }
            catch (Exception exc)
            {
                await logWorker.LogError(ControlledAction, $"{exc.Source} - Controller:{ControlledContext}", exc.Message, exc.GetType().ToString());
                throw new Exception($"Something went wrong with:{actionNames}");
            }
        }
        [HttpGet]
        [Route("GetApplicationExistanceCheck")]
        public async Task<IActionResult> GetApplicationExistanceCheck(string Name)
        {
            var actionNames = GetControllerActionNames();
            try
            {
                var result = await applicationWorker.VerifyApplicationExistance(Name);

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

        private string GetControllerActionNames()
        {
            ControlledContext = ControllerContext.ActionDescriptor.ControllerName;
            ControlledAction = ControllerContext.ActionDescriptor.ActionName;
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
