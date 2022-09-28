namespace Portfolio_AppRepo_API.ViewModels
{
    public class ApplicationViewModel
    {
        public int ID { get; set; }
        public string? URL { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? User { get; set; }
        public bool IsActive { get; set; }
        public string? Stage { get; set; }
        public List<ApiUrlViewModel> ApiList { get; set; } = new List<ApiUrlViewModel>();
        public List<ApiUrlViewModel> ServiceList { get; set; } = new List<ApiUrlViewModel>();
        public FileNameViewModel FileNameModel { get; set; } = new FileNameViewModel();
    }

}
