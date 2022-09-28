using System;
using System.Collections.Generic;

namespace Portfolio_AppRepo_API.Models
{
    public partial class Application
    {
        public int Id { get; set; }
        public string? Url { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? User { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastModified { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public string? Stage { get; set; }
    }
}
