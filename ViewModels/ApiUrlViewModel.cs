namespace Portfolio_AppRepo_API.ViewModels
{
    public class ApiUrlViewModel
    {
        public int DevId { get; set; }
        public int UatId { get; set; }
        public int ProdId { get; set; }
        public string? Name { get; set; }
        public bool status { get; set; } = true;
        public string? Dev { get; set; }
        public string? Uat { get; set; }
        public string? Prod { get; set; }
        public bool IsDelete { get; set; }
    }
}
