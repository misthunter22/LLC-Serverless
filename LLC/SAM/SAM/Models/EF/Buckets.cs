using System;
using System.Collections.Generic;

namespace DbCore.Models
{
    public partial class Buckets
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }
        public string SearchPrefix { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
