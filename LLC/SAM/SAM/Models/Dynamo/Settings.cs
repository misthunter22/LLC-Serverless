using Amazon.DynamoDBv2.DataModel;
using SAM.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM.Models.Dynamo
{
    [DynamoDBTable("LLC-Settings")]
    public class Settings
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        private string _dateCreated;

        [DynamoDBProperty]
        public string DateCreated
        {
            get { return _dateCreated; }
            set { _dateCreated = StringHelper.ParseDate(value); }
        }

        private string _dateModified;

        [DynamoDBProperty]
        public string DateModified
        {
            get { return _dateModified; }
            set { _dateModified = StringHelper.ParseDate(value); }
        }

        [DynamoDBProperty]
        public string Description { get; set; }

        [DynamoDBProperty]
        public string ModifiedUser { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public string Value { get; set; }

        [DynamoDBVersion]
        public int? VersionNumber { get; set; }
    }
}
