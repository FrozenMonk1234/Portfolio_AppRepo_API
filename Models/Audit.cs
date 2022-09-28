using System;
using System.Collections.Generic;

namespace Portfolio_AppRepo_API.Models
{
    public partial class Audit
    {
        public int Id { get; set; }
        public string? User { get; set; }
        public string? Action { get; set; }
        public DateTime? Date { get; set; }
    }
}
