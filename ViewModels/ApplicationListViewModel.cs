namespace Portfolio_AppRepo_API.ViewModels
{
    public class ApplicationListViewModel
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public int CountApi { get; set; }
        public int CountService { get; set; }
        public string? Stage { get; set; }
        public bool IsActive { get; set; }

    }
}
