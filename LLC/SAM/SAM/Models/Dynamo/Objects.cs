using Amazon.DynamoDBv2.DataModel;
using SAM.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace SAM.Models.Dynamo
{
    [DynamoDBTable("LLC-Objects")]
    public class Objects
    {
        [DynamoDBHashKey]
        public BigInteger Id { get; set; }

        [DynamoDBProperty]
        public int? Bucket { get; set; }

        [DynamoDBProperty]
        public string ContentLastModified { get; set; }

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
