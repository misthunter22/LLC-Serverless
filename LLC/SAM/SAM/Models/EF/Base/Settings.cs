using System;
using System.Collections.Generic;

namespace DbCore.Models
{
    public partial class Settings
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string ModifiedUser { get; set; }
    }
}
