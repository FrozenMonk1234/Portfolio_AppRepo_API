using System;
using System.Collections.Generic;

namespace Portfolio_AppRepo_API.Models
{
    public partial class ApplicationFile
    {
        public int Id { get; set; }
        public int? ApplicationId { get; set; }
        public bool ProjectProposal { get; set; }
        public bool BusinessCase { get; set; }
        public bool BusinessRequirement { get; set; }
        public bool TechnicalSpecification { get; set; }
        public bool TestCase { get; set; }
        public bool UserManual { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
