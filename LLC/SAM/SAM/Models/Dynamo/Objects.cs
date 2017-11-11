using Amazon.DynamoDBv2.DataModel;

namespace SAM.Models.Dynamo
{
    [DynamoDBTable("LLC-Objects")]
    public class Objects
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        [DynamoDBProperty]
        public string Bucket { get; set; }

        [DynamoDBProperty]
        public string ContentLastModified { get; set; }

        [DynamoDBProperty]
        public string DateLinksLastExtracted { get; set; }

        [DynamoDBProperty]
        public string DateFirstFound { get; set; }

        [DynamoDBProperty]
        public string DateLastFound { get; set; }

        [DynamoDBProperty]
        public string ETag { get; set; }

        [DynamoDBProperty]
        public bool IsFolder { get; set; }

        [DynamoDBProperty]
        public string ItemName { get; set; }

        [DynamoDBProperty]
        public string Key { get; set; }

        [DynamoDBVersion]
        public int? VersionNumber { get; set; }
    }
}
