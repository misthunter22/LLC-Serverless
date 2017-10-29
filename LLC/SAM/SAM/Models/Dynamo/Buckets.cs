using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM.Models.Dynamo
{
    [DynamoDBTable("LLC-Buckets")]
    public class Buckets
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        [DynamoDBProperty]
        public string AccessKey { get; set; }

        [DynamoDBProperty]
        public string DateCreated { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public string Region { get; set; }

        [DynamoDBProperty]
        public string SearchPrefix { get; set; }

        [DynamoDBProperty]
        public string SecretKey { get; set; }

        [DynamoDBVersion]
        public int? VersionNumber { get; set; }
    }
}
