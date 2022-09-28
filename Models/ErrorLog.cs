using System;
using System.Collections.Generic;

namespace Portfolio_AppRepo_API.Models
{
    public partial class ErrorLog
    {
        public int Id { get; set; }
        public string? ExceptionType { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? InnerException { get; set; }
        public string? ExceptionSource { get; set; }
        public DateTime? Date { get; set; }
    }
}
