using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAM.DI;
using System;

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
            var n      = record["s3"]["bucket"]["name"].ToString();
            var k      = record["s3"]["object"]["key"].ToString();
            var evt    = record["eventName"].ToString();
            var isPut  = "ObjectCreated:Put".Equals(evt);

            Console.WriteLine(evt);
            Console.WriteLine(n);
            Console.WriteLine(k);

            // Get the source
            long ret = -1;
            var source = service.Source(n, Models.Admin.SourceSearchType.Name);
            if (source != null)
            {
                // Compute the list of exclusions
                var isMobile = k.IndexOf("mobile_pages") >= 0;
                var prefix   = source.S3BucketSearchPrefix;
                if (!prefix.EndsWith("/"))
                    prefix = prefix + "/";

                // Only process if it meets the criteria
                if (isPut && !isMobile && k.StartsWith(prefix))
                {
                    // Set the object in the table on 
                    // the create action

                }

                // Update the table with the latest S3 count
                var count = service.TableCount("LLC-Objects");
                ret = service.SetMetaTableKey("Objects", count).Result;
            }
            else
            {
                Console.WriteLine($"Could not find source {n} by name");
            }

            return ret;
        }
    }
}
