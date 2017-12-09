using DbCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DbCore.Models
{
    public class SourcesExt
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public bool? AllowLinkChecking { get; set; }

        public bool? AllowLinkExtractions { get; set; }

        public string S3bucketId { get; set; }

        public DateTime? DateCreated { get; set; }

        [Required]
        public string S3bucketSearchPrefix { get; set; }

        [Required]
        public string S3bucketName { get; set; }

        [Required]
        public string Region { get; set; }

        [Required]
        public string AccessKey { get; set; }

        [Required]
        public string SecretKey { get; set; }

        public bool Delete { get; set; }
    }
}
