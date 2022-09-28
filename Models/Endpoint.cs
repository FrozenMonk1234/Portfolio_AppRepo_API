using System;
using System.Collections.Generic;

namespace Portfolio_AppRepo_API.Models
{
    public partial class Endpoint
    {
        public int Id { get; set; }
        public int? ApplicationId { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? Stage { get; set; }
        public string? Type { get; set; }
        public string? User { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
    }
}
