using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM.Models.Dynamo
{
    [DynamoDBTable("LLC-Reports")]
    public class WarningLinks
    {
        [DynamoDBHashKey]
        public int Id { get; set; }

        [DynamoDBProperty]
        public int ContentSize { get; set; }

        [DynamoDBProperty]
        public int Mean { get; set; }

        [DynamoDBProperty]
        public long StandardDeviation { get; set; }

        [DynamoDBProperty]
        public int SdRange { get; set; }

        [DynamoDBProperty]
        public int LinkId { get; set; }

        [DynamoDBProperty]
        public int StatId { get; set; }

        [DynamoDBProperty]
        public string Url { get; set; }

        [DynamoDBVersion]
        public int? VersionNumber { get; set; }
    }
}
