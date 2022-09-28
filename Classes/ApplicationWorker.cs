using Portfolio_AppRepo_API.Models;
using Portfolio_AppRepo_API.Repository.ApplicationFileRepo;
using Portfolio_AppRepo_API.Repository.ApplicationRepo;
using Portfolio_AppRepo_API.Repository.EndpointRepo;
using Portfolio_AppRepo_API.ViewModels;

namespace Portfolio_AppRepo_API.Classes
{
    sealed class ApplicationWorker
    {
        private readonly IApplicationRepo _applicationRepo;
        private readonly IEndpointRepo _endpointRepo;
        private readonly IApplicationFileRepo _applicationFileRepo;
        public ApplicationWorker(IApplicationRepo applicationRepo, IEndpointRepo endpointRepo, IApplicationFileRepo applicationFileRepo)
        {
            _applicationRepo = applicationRepo;
            _endpointRepo = endpointRepo;
            _applicationFileRepo = applicationFileRepo;
        }

        internal async Task<bool> UpdateApplicationAsync(ApplicationViewModel model)
        {
            var date = DateTime.Now;
            var appModel = await TransposeApplicationEntry(model);
            appModel.LastModified = date;
            var executed = await _applicationRepo.Update(appModel);
            if (executed)
            {
                var fileModel = await TranposeApplicationFileEntries(model.FileNameModel, model.ID);
                fileModel.DateModified = date;
                await _applicationFileRepo.Update(fileModel);

                var endpointList = await TransposeApplicationEndpointEntries(model, model.ID, "api");
                endpointList.AddRange(await TransposeApplicationEndpointEntries(model, model.ID, "service"));
                if (endpointList.Any())
                {
                    await _endpointRepo.DeleteRangeByApplictionId(model.ID);

                    foreach (var endpoint in endpointList)
                    {
                        endpoint.DateModified = date;
                    }
                    var updateEnpointResult = await _endpointRepo.CreateRange(endpointList);
                    return updateEnpointResult;
                }
            }
            return false;
        }

        internal async Task<int> CreateAppliactionAsync(ApplicationViewModel model)
        {
            var date = DateTime.Now;
            var appModel = await TransposeApplicationEntry(model);
            appModel.DateCreated = date;
            var applicationId = await _applicationRepo.Create(appModel);
            if (applicationId > 0)
            {
                var fileModel = await TranposeApplicationFileEntries(model.FileNameModel, applicationId);
                fileModel.DateCreated = date;
                var fileResult = await _applicationFileRepo.Create(fileModel);
                if (!fileResult)
                {
                    throw new Exception("Something went wrong while saving file records.");
                }

                var endpointList = new List<Models.Endpoint>();
                if (model.ApiList.Any())
                {
                    endpointList.AddRange(await TransposeApplicationEndpointEntries(model, applicationId, "api"));
                }
                if (model.ServiceList.Any())
                {
                    endpointList.AddRange(await TransposeApplicationEndpointEntries(model, applicationId, "service"));
                }
                if (endpointList.Any())
                {
                    foreach (var endpoint in endpointList)
                    {
                        endpoint.DateCreated = date;
                    }
                    if (await _endpointRepo.CreateRange(endpointList) == false)
                    {
                        throw new Exception("Something went wrong while creating enpoints.");
                    }
                }
            }
            return applicationId;
        }

        internal async Task<ApplicationViewModel> GetApplicationById(int Id)
        {
            var result = new ApplicationViewModel();
            var model = await _applicationRepo.GetById(Id);

            if (model != null)
            {
                result.Name = model.Name;
                result.User = model.User;
                result.URL = model.Url;
                result.Description = model.Description;
                result.ID = model.Id;
                result.Stage = model.Stage;
                result.IsActive = (bool)model.IsActive;

                var apiList = await GetEndpointListByIdAndType(Id, "api");
                if (apiList != null)
                {
                    result.ApiList.AddRange(apiList);
                }

                var serviceList = await GetEndpointListByIdAndType(Id, "service");
                if (serviceList != null)
                {
                    result.ServiceList.AddRange(serviceList);
                }

                var fileNames = GetApplicationFileNamesByApplicationId(Id);
                if (fileNames != null)
                {
                    result.FileNameModel = fileNames.Result;
                }
            }
            else
            {
                throw new Exception($"No data found for ID:{Id}");
            }
            return result;
        }

        internal async Task<List<ApplicationViewModel>> GetAllApplications()
        {
            List<ApplicationViewModel> result = new List<ApplicationViewModel>();
            var appList = await _applicationRepo.GetAll();
            if (appList != null)
            {

                foreach (var app in appList)
                {
                    ApplicationViewModel model = new ApplicationViewModel();
                    model.Name = app.Name;
                    model.User = app.User;
                    model.URL = app.Url;
                    model.Description = app.Description;
                    model.ID = app.Id;
                    model.IsActive = (bool)app.IsActive;
                    model.Stage = app.Stage;

                    var apiList = await GetEndpointListByIdAndType(app.Id, "api");
                    if (apiList != null)
                    {
                        model.ApiList.AddRange(apiList);
                    }

                    var serviceList = await GetEndpointListByIdAndType(app.Id, "service");
                    if (serviceList != null)
                    {
                        model.ServiceList.AddRange(serviceList);
                    }

                    var fileNames = GetApplicationFileNamesByApplicationId(app.Id);
                    if (fileNames != null)
                    {
                        model.FileNameModel = fileNames.Result;
                    }

                    result.Add(model);
                }
            }
            return result;
        }

        internal async Task<bool> DeleteApplicationById(int id, string user)
        {
            var date = DateTime.Now;
            var appModel = await _applicationRepo.GetById(id);
            if (appModel != null)
            {
                appModel.IsDelete = true;
                appModel.User = user;
                appModel.LastModified = date;
                var appUpdateResult = await _applicationRepo.Update(appModel);
                if (appUpdateResult)
                {
                    var endpointList = await _endpointRepo.GetAllByApplicationId(appModel.Id);
                    if (!endpointList.Any())
                    {
                        foreach (var endpoint in endpointList)
                        {
                            endpoint.IsDelete = true;
                            endpoint.User = user;
                            endpoint.DateModified = date;
                        }

                        await _endpointRepo.UpdateRange(endpointList);
                    }
                }
                return appUpdateResult;
            }
            else
            {
                throw new Exception("Record not found.");
            }


        }

        internal async Task<List<Application>> VerifyApplicationExistance(string Name)
        {
            var result = new List<Application>();
            var allEntries = await _applicationRepo.GetAll();
            result = allEntries.Where(x => x.Name.ToLower().Trim().Contains(Name.ToLower().Trim())).ToList();
            return result;
        }

        #region Get Application Methods
        private async Task<List<ApiUrlViewModel>> GetEndpointListByIdAndType(int Id, string type)
        {
            List<ApiUrlViewModel> result = new List<ApiUrlViewModel>();
            var endpoints = await _endpointRepo.GetAllByApplicationId(Id);
            if (endpoints != null)
            {
                var group = endpoints.Where(x => x.Type.ToLower() == type.ToLower()).GroupBy(x => x.Name).ToList();

                foreach (var item in group)
                {
                    var S = item.FirstOrDefault().IsActive.Value;
                    var K = item.Key;
                    var U = "";
                    var P = "";
                    var I = item.FirstOrDefault().Id;
                    var D = "";
                    var isDelete = item.FirstOrDefault().IsDelete.Value;

                    if (item.FirstOrDefault(x => x.Stage.ToLower() == "dev").IsActive == true)
                    {
                        D = item.FirstOrDefault(x => x.Stage.ToLower() == "dev").Url;
                    }
                    if (item.FirstOrDefault(x => x.Stage.ToLower() == "uat").IsActive == true)
                    {
                        U = item.FirstOrDefault(x => x.Stage.ToLower() == "uat").Url;
                    }
                    if (item.FirstOrDefault(x => x.Stage.ToLower() == "prod").IsActive == true)
                    {
                        P = item.FirstOrDefault(x => x.Stage.ToLower() == "prod").Url;
                    }

                    result.Add(new ApiUrlViewModel()
                    {
                        DevId = item.FirstOrDefault(x => x.Stage.ToLower() == "dev").Id,
                        UatId = item.FirstOrDefault(x => x.Stage.ToLower() == "uat").Id,
                        ProdId = item.FirstOrDefault(x => x.Stage.ToLower() == "prod").Id,
                        Name = $"{K}",
                        Dev = $"{D}",
                        Uat = $"{U}",
                        Prod = $"{P}",
                        status = S,
                        IsDelete = isDelete
                    });
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        private async Task<FileNameViewModel> GetApplicationFileNamesByApplicationId(int Id)
        {
            var fileNames = await _applicationFileRepo.GetByApplicationId(Id);
            return new FileNameViewModel()
            {
                Id = fileNames.Id,
                BusinessCase = fileNames.BusinessCase,
                BusinessRequirement = fileNames.BusinessRequirement,
                ProjectProposal = fileNames.ProjectProposal,
                TechnicalSpecification = fileNames.TechnicalSpecification,
                TestCase = fileNames.TestCase,
                UserManual = fileNames.UserManual
            };
        }
        #endregion

        #region Transpose Application Models to Db Models
        private Task<Application> TransposeApplicationEntry(ApplicationViewModel model)
        {
            Application applicationModel = new Application();
            applicationModel.Id = model.ID;
            applicationModel.Name = model.Name;
            applicationModel.Description = model.Description;
            applicationModel.IsActive = model.IsActive;
            applicationModel.Url = model.URL;
            applicationModel.User = model.User;
            applicationModel.DateCreated = DateTime.Now;
            applicationModel.IsActive = model.IsActive;
            applicationModel.IsDelete = false;
            applicationModel.Stage = model.Stage;
            return Task.FromResult(applicationModel);
        }
        private Task<List<Models.Endpoint>> TransposeApplicationEndpointEntries(ApplicationViewModel model, int applicationId, string type)
        {
            List<Models.Endpoint> list = new List<Models.Endpoint>();

            List<ApiUrlViewModel> endpointList = new List<ApiUrlViewModel>();

            if (type.ToLower() == "api")
            {
                endpointList = model.ApiList;
            }
            else if (type.ToLower() == "service")
            {
                endpointList = model.ServiceList;
            }

            foreach (var item in endpointList)
            {
                var isActive = true;
                if (string.IsNullOrEmpty(item.Dev)) { isActive = false; }
                list.Add(new Models.Endpoint
                {
                    Id = item.DevId,
                    ApplicationId = applicationId,
                    DateCreated = DateTime.Now,
                    IsActive = isActive,
                    IsDelete = item.IsDelete,
                    Stage = "dev",
                    User = model.User,
                    Name = item.Name,
                    Type = type,
                    Url = $"{item.Dev}"
                });
                isActive = true;
                if (string.IsNullOrEmpty(item.Uat)) { isActive = false; }
                list.Add(new Models.Endpoint
                {
                    Id = item.UatId,
                    ApplicationId = applicationId,
                    DateCreated = DateTime.Now,
                    IsActive = isActive,
                    IsDelete = item.IsDelete,
                    Stage = "uat",
                    User = model.User,
                    Name = item.Name,
                    Type = type,
                    Url = $"{item.Uat}"
                });
                isActive = true;
                if (string.IsNullOrEmpty(item.Prod)) { isActive = false; }
                list.Add(new Models.Endpoint
                {
                    Id = item.ProdId,
                    ApplicationId = applicationId,
                    DateCreated = DateTime.Now,
                    IsActive = isActive,
                    IsDelete = item.IsDelete,
                    Stage = "prod",
                    User = model.User,
                    Name = item.Name,
                    Type = type,
                    Url = $"{item.Prod}"
                });
            }
            return Task.FromResult(list);
        }
        private Task<ApplicationFile> TranposeApplicationFileEntries(FileNameViewModel model, int applicationId)
        {
            ApplicationFile applicationFileModel = new ApplicationFile();
            applicationFileModel.Id = model.Id;
            applicationFileModel.ApplicationId = applicationId;
            applicationFileModel.DateCreated = DateTime.Now;
            applicationFileModel.BusinessRequirement = model.BusinessRequirement;
            applicationFileModel.UserManual = model.UserManual;
            applicationFileModel.BusinessCase = model.BusinessCase;
            applicationFileModel.ProjectProposal = model.ProjectProposal;
            applicationFileModel.TechnicalSpecification = model.TechnicalSpecification;
            applicationFileModel.TestCase = model.TestCase;

            return Task.FromResult(applicationFileModel);
        }
        #endregion
    }
}
//int recordCount = list.Count;
//int recordSaved = 0;
//foreach (var endpoint in list)
//{
//    var endpointToSave = new Models.Endpoint()
//    {
//        ApplicationId = endpoint.ApplicationId,
//        DateCreated = endpoint.DateCreated,
//        DateModified = endpoint.DateModified,
//        IsActive = endpoint.IsActive,
//        IsDelete = endpoint.IsDelete,
//        Name = endpoint.Name,
//        Stage = endpoint.Stage,
//        Type = endpoint.Type,
//        Url = endpoint.Url,
//        User = endpoint.User,
//    };
//    var endpointSaveResult = await _endpointRepo.Update(endpoint);
//    if (endpointSaveResult)
//        recordSaved++;
//    else
//        throw new Exception($"Something went wrong.{recordSaved}/{recordCount} Endpoints Saved ");
//}