using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM.Models.Dynamo
{
    [DynamoDBTable("LLC-Meta")]
    public class Meta
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        [DynamoDBProperty]
        public int Key { get; set; }

        [DynamoDBVersion]
        public int? VersionNumber { get; set; }
    }
}
