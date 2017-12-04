using System;
using System.Collections.Generic;

namespace DbCore.Models
{
    public partial class Sources
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? AllowLinkChecking { get; set; }
        public bool? AllowLinkExtractions { get; set; }
        public string S3bucketId { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
