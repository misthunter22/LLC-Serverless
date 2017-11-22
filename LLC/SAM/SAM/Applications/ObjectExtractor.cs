using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAM.Models.Dynamo;
using System;

namespace SAM.Applications
{
    public class ObjectExtractor : BaseHandler
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public void Handler(object input, ILambdaContext context)
        {
            Console.WriteLine(input.GetType());
            JObject obj = (JObject)input;

            // Determine the type of event (+/-)
            var record = obj["Records"][0];
            var k      = record["s3"]["object"]["key"].ToString();
            var evt    = record["eventName"].ToString();
            var isPut  = evt.StartsWith("ObjectCreated:");

            Console.WriteLine(evt);
            Console.WriteLine(k);

            // Compute the list of exclusions
            var isMobile = k.IndexOf("mobile_pages") >= 0;

            // Only process if it meets the criteria
            if (isPut && !isMobile)
            {
                var e = record["s3"]["object"]["eTag"].ToString();
                var n = record["s3"]["bucket"]["name"].ToString();

                var source = Service.Source(n, Models.Admin.SourceSearchType.Name);
                if (source == null)
                {
                    Console.WriteLine($"Could not find source {n} by name");
                    return;
                }

                var prefix = source.S3BucketSearchPrefix;
                if (!prefix.EndsWith("/"))
                    prefix = prefix + "/";

                Console.WriteLine($"Found source {source.Name}");
                Console.WriteLine($"Source prefix is {source.S3BucketSearchPrefix}");

                if (!k.StartsWith(prefix))
                    return;

                Console.WriteLine("Prefix matches");

                // Set the object in the table on 
                // the create action
                var newId       = Guid.NewGuid();
                var date        = DateTime.Now.ToString();
                var keySplit    = k.Split('/');
                var itemName    = keySplit[keySplit.Length - 1];
                var existingRow = Service.GetTableQuery<Objects>("Key", k, "KeyIndex");
                Console.WriteLine($"New ID is : {newId}");
                var newRow      = new Objects
                {
                    Bucket = source.S3BucketId,
                    ContentLastModified = date,
                    DateLastFound = date,
                    ETag = e,
                    IsFolder = k.EndsWith("/"),
                    ItemName = itemName,
                    Key = k
                };

                if (existingRow.Count == 0)
                {
                    newRow.Id = newId.ToString();
                    newRow.DateFirstFound = date;
                    Console.WriteLine($"Adding new item with ID: {newRow.Id}");
                }
                else
                {
                    newRow = Service.Object(existingRow[0].Id);
                    newRow.ContentLastModified = date;
                    newRow.DateLastFound = date;
                    newRow.ETag = e;
                    Console.WriteLine($"Using existing ID: {newRow.Id}");
                }

                Console.WriteLine($"Object is: {JsonConvert.SerializeObject(newRow)}");
                var result = Service.SetTableRow(newRow).Result;

                LinkExtractions(newRow, n, source.Id);
            }
        }
    }
}
