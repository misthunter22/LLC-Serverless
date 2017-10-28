using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAM.DI;
using SAM.Models.Dynamo;
using System;
using System.Numerics;

namespace SAM.Applications
{
    public class ObjectExtractor : BaseHandler
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public long Handler(object input, ILambdaContext context)
        {
            Console.WriteLine(JsonConvert.SerializeObject(input));
            Console.WriteLine(input.GetType());
            JObject obj = (JObject)input;

            var service = new ILLCDataImpl();

            // Determine the type of event (+/-)
            var record = obj["Records"][0];
            var k      = record["s3"]["object"]["key"].ToString();
            var evt    = record["eventName"].ToString();
            var isPut  = evt.StartsWith("ObjectCreated:");

            Console.WriteLine(evt);
            Console.WriteLine(k);

            // Get the source
            long ret = -1;

            // Compute the list of exclusions
            var isMobile = k.IndexOf("mobile_pages") >= 0;

            // Only process if it meets the criteria
            if (isPut && !isMobile)
            {
                var e = record["s3"]["object"]["eTag"].ToString();
                var n = record["s3"]["bucket"]["name"].ToString();

                var source = service.Source(n, Models.Admin.SourceSearchType.Name);
                if (source != null)
                {
                    var prefix = source.S3BucketSearchPrefix;
                    if (!prefix.EndsWith("/"))
                        prefix = prefix + "/";

                    Console.WriteLine($"Found source {source.Name}");
                    Console.WriteLine($"Source prefix is {source.S3BucketSearchPrefix}");

                    if (k.StartsWith(prefix))
                    {
                        Console.WriteLine("Prefix matches");

                        // Set the object in the table on 
                        // the create action
                        var newId       = new Guid();
                        var date        = DateTime.Now.ToString();
                        var keySplit    = k.Split('/');
                        var itemName    = keySplit[keySplit.Length - 1];
                        var existingRow = TableQuery(k, service);
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

                        if (existingRow.Key != null)
                        {
                            newRow.Id = new BigInteger(newId.ToByteArray());
                            newRow.DateFirstFound = date;
                            Console.WriteLine(newRow.Id.ToString());
                        }

                        var result = service.SetTableRow(newRow).Result;

                        // Update the table with the latest S3 count
                        var count = service.TableCount("LLC-Objects");
                        ret = service.SetMetaTableKey("Objects", count).Result;
                    }
                }
                else
                {
                    Console.WriteLine($"Could not find source {n} by name");
                }
            }

            return ret;
        }

        private Objects TableQuery(string key, ILLCData service)
        {
            var obj = service.QueryDataAttributes("LLC-Objects", new AttributeValue { S = key }, "Key", "KeyIndex").Result;
            if (obj == null)
                return new Objects();

            return new Objects
            {
                Bucket              = obj.ContainsKey("Bucket")              ? StringHelper.ParseInt(obj["Bucket"].N)        : (int?)null,
                ContentLastModified = obj.ContainsKey("ContentLastModified") ? obj["ContentLastModified"].S                  : null,
                DateFirstFound      = obj.ContainsKey("DateFirstFound")      ? obj["DateFirstFound"].S                       : null,
                DateLastFound       = obj.ContainsKey("DateLastFound")       ? obj["DateLastFound"].S                        : null,
                ETag                = obj.ContainsKey("ETag")                ? obj["ETag"].S                                 : null,
                Id                  = obj.ContainsKey("Id")                  ? BigInteger.Parse(obj["Id"].N.ToString())      : -1,
                IsFolder            = obj.ContainsKey("IsFolder")            ? obj["IsFolder"].BOOL                          : false,
                ItemName            = obj.ContainsKey("ItemName")            ? obj["ItemName"].S                             : null,
                Key                 = obj.ContainsKey("Key")                 ? obj["Key"].S                                  : null,
                VersionNumber       = obj.ContainsKey("VersionNumber")       ? StringHelper.ParseInt(obj["VersionNumber"].N) : (int?)null
            };
        }
    }
}
