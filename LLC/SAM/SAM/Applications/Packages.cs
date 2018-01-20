using Amazon.Lambda.Core;
using DbCore.Models;
using Newtonsoft.Json.Linq;
using System;

namespace SAM.Applications
{
    public class Pckgs : BaseHandler
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
            var isPut  = evt.StartsWith("ObjectCreated:", StringComparison.CurrentCultureIgnoreCase);
            var isCopy = evt.EndsWith("Copy", StringComparison.CurrentCultureIgnoreCase);
            var isDel  = evt.StartsWith("ObjectRemoved:", StringComparison.CurrentCultureIgnoreCase);

            Console.WriteLine(evt);
            Console.WriteLine(k);

            var t = record["s3"]["object"]["eTag"];
            var e = t != null ? t.ToString() : string.Empty;
            var b = record["s3"]["bucket"]["name"].ToString();

            // Only process if it meets the criteria
            if (isPut || isCopy)
            {
                var o = Service.ObjectGet(b, k);
                var d = string.Empty;
                var n = string.Empty;

                if (o.Metadata != null)
                {
                    d = o.Metadata["x-amz-meta-description"];
                    n = o.Metadata["x-amz-meta-name"];
                }

                if (isPut && !isCopy)
                {
                    // Set the object in the table on 
                    // the create action
                    var newId = Guid.NewGuid().ToString();
                    var date = DateTime.Now;
                    var keySplit = k.Split('/');
                    var itemName = keySplit[keySplit.Length - 1];
                    var existingRow = Service.ObjectFromKey(k);
                    Console.WriteLine($"New ID is : {newId}");
                    var newRow = new Packages
                    {
                        DateUploaded = date,
                        Description = d,
                        FileName = k.Split('/')[k.Split('/').Length - 1],
                        Id = newId,
                        Key = k,
                        Name = n,
                        PackageProcessed = false
                    };

                    Service.AddPackage(newRow);

                    // Process package
                }
                else if (isCopy)
                {
                    var package = Service.PackageFromKey(k);
                    package.Name = n;
                    package.Description = d;
                    Service.SavePackage(package);
                }
            }
            else if (isDel)
            {
                Service.DeletePackage(k);
            }
        }
    }
}
