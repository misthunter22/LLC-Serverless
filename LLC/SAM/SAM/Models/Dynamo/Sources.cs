using Amazon.DynamoDBv2.DataModel;
using SAM.DI;
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

        private string _dateCreated;

        [DynamoDBProperty]
        public string DateCreated
        {
            get { return _dateCreated; }
            set { _dateCreated = StringHelper.ParseDate(value); }
        }

        private string _dateLastExtracted;

        [DynamoDBProperty]
        public string DateLastExtracted
        {
            get { return _dateLastExtracted; }
            set { _dateLastExtracted = StringHelper.ParseDate(value); }
        }

        private string _dateLastChecked;

        [DynamoDBProperty]
        public string DateLastChecked
        {
            get { return _dateLastChecked; }
            set { _dateLastChecked = StringHelper.ParseDate(value); }
        }

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
