using System;
using System.Collections.Generic;

namespace DbCore.Models
{
    public partial class Sources
    {
        public string S3ObjectName { get;set; }
        public string S3BucketSearchPrefix { get; set; }
    }
}
