using Amazon.DynamoDBv2.DataModel;

namespace SAM.Models.Dynamo
{
    [DynamoDBTable("LLC-Links")]
    public class Links
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        [DynamoDBProperty]
        public decimal AllTimeMaxDownloadTime { get; set; }

        [DynamoDBProperty]
        public decimal AllTimeMinDownloadTime { get; set; }

        [DynamoDBProperty]
        public decimal AllTimeStdDevDownloadTime { get; set; }

        [DynamoDBProperty]
        public int AttemptCount { get; set; }

        [DynamoDBProperty]
        public string DateFirstFound { get; set; }

        [DynamoDBProperty]
        public string DateLastChecked { get; set; }

        [DynamoDBProperty]
        public string DateLastFound { get; set; }

        [DynamoDBProperty]
        public string DateUpdated { get; set; }

        [DynamoDBProperty]
        public decimal PastWeekMaxDownloadTime { get; set; }

        [DynamoDBProperty]
        public decimal PastWeekMinDownloadTime { get; set; }

        [DynamoDBProperty]
        public decimal PastWeekStdDevDownloadTime { get; set; }

        [DynamoDBProperty]
        public string Source { get; set; }

        [DynamoDBProperty]
        public string Url { get; set; }

        [DynamoDBProperty]
        public bool Valid { get; set; }

        [DynamoDBVersion]
        public int? VersionNumber { get; set; }
    }
}
