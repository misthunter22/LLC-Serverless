using DbCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbCore.Models
{
    public class SourcesExt
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? AllowLinkChecking { get; set; }
        public bool? AllowLinkExtractions { get; set; }
        public string S3bucketId { get; set; }
        public DateTime? DateCreated { get; set; }

        public string S3bucketSearchPrefix { get; set; }

        public string S3bucketName { get; set; }
    }
}
