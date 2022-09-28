using Microsoft.AspNetCore.Components.Forms;

namespace Portfolio_AppRepo_API.ViewModels
{
    public class FileUploadViewModel
    {
        public string? ApplicationName { get; set; }
        public string? DocumentType { get; set; }
        public IBrowserFile? UploadedFile { get; set; }
    }
}
