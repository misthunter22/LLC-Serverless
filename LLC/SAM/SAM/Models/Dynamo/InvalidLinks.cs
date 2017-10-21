using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM.Models.Dynamo
{
    [DynamoDBTable("LLC-Reports")]
    public class InvalidLinks
    {
        [DynamoDBHashKey]
        public int Id { get; set; }

        [DynamoDBProperty]
        public int Link { get; set; }

        [DynamoDBProperty]
        public int AttemptCount { get; set; }

        [DynamoDBProperty]
        public string DateLastFound { get; set; }

        [DynamoDBProperty]
        public string Source { get; set; }

        [DynamoDBProperty]
        public string Url { get; set; }

        [DynamoDBProperty]
        public string DateLastChecked { get; set; }

        [DynamoDBVersion]
        public int? VersionNumber { get; set; }
    }
}
