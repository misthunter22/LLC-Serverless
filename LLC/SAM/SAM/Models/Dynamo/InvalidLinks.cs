using Amazon.DynamoDBv2.DataModel;
using SAM.DI;
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
        public string Id { get; set; }

        [DynamoDBProperty]
        public string Link { get; set; }

        [DynamoDBProperty]
        public int AttemptCount { get; set; }

        private string _dateLastFound;

        [DynamoDBProperty]
        public string DateLastFound
        {
            get { return _dateLastFound; }
            set { _dateLastFound = StringHelper.ParseDate(value); }
        }

        [DynamoDBProperty]
        public string Source { get; set; }

        [DynamoDBProperty]
        public string Url { get; set; }

        private string _dateLastChecked;

        [DynamoDBProperty]
        public string DateLastChecked
        {
            get { return _dateLastChecked; }
            set { _dateLastChecked = StringHelper.ParseDate(value); }
        }

        [DynamoDBVersion]
        public int? VersionNumber { get; set; }
    }
}
