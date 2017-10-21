using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM.Models.Dynamo
{
    [DynamoDBTable("LLC-Sources")]
    public class Sources
    {
        [DynamoDBHashKey]
        public int Id { get; set; }

        [DynamoDBProperty]
        public bool AllowLinkChecking { get; set; }

        [DynamoDBProperty]
        public bool AllowLinkExtractions { get; set; }

        [DynamoDBProperty]
        public string DateCreated { get; set; }

        [DynamoDBProperty]
        public string DateLastChecked { get; set; }

        [DynamoDBProperty]
        public string DateLastExtracted { get; set; }

        [DynamoDBProperty]
        public string Description { get; set; }

        [DynamoDBProperty]
        public int? HtmlFileCount { get; set; }

        [DynamoDBProperty]
        public int? InvalidLinkCount { get; set; }

        [DynamoDBProperty]
        public int? LinkCount { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public int? S3BucketId { get; set; }

        [DynamoDBProperty]
        public string S3BucketSearchPrefix { get; set; }

        [DynamoDBProperty]
        public int? S3ObjectCount { get; set; }

        [DynamoDBProperty]
        public string S3ObjectName { get; set; }

        [DynamoDBVersion]
        public int? VersionNumber { get; set; }
    }
}
