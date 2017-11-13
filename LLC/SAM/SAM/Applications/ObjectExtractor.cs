using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAM.Models.Dynamo;
using System;
using System.IO;
using System.Text.RegularExpressions;

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

                // Peform any link extractions
                var content = Service.ObjectGet(n, k);

                // Find any links 
                if (content == null)
                    return;

                Console.WriteLine("Found S3 content");

                var reader = new StreamReader(content.ResponseStream);
                var text   = reader.ReadToEnd();

                MatchCollection matchList = R.Matches(text);

                if (matchList.Count > 0)
                {
                    Console.WriteLine("Found HTML Regex matches");

                    // One or more links were found so we'll include each in the bulk update
                    foreach (Match m in matchList)
                    {
                        // Allow send through URLs up to 1024 in length to avoid errors
                        var url = m.Groups[1].Value;
                        url = (url.Length >= MaxUrlLength ? url.Substring(0, MaxUrlLength - 1) : url);
                        Console.WriteLine($"Found URL: {url}");

                        var row = new Links
                        {
                            Id = Guid.NewGuid().ToString(),
                            DateLastFound = date,
                            Source = source.Id,
                            Url = url
                        };

                        var existingLink = Service.GetTableQuery<Links>("Url", url, "UrlIndex");
                        if (existingLink.Count == 0)
                        {
                            row.DateFirstFound = date;
                        }
                        else
                        {
                            row = existingLink[0];
                            row.DateLastFound = date;
                        }

                        var r = Service.SetTableRow(row).Result;
                        Console.WriteLine(JsonConvert.SerializeObject(r));
                    }
                }
            }
        }
    }
}
