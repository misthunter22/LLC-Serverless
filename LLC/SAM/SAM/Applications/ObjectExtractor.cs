using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAM.DI;
using System;
using System.Linq;

namespace SAM.Applications
{
    public class ObjectExtractor : BaseHandler
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public int Handler(object input, ILambdaContext context)
        {
            Console.WriteLine(JsonConvert.SerializeObject(input));
            Console.WriteLine(input.GetType());
            JObject obj = (JObject)input;

            var service = new ILLCDataImpl();

            // Determine the type of event (+/-)
            var record = obj["Records"][0];
            var n      = record["s3"]["bucket"]["name"].ToString();
            var k      = record["s3"]["object"]["key"].ToString();
            var v      = record["s3"]["object"]["versionId"].ToString();
            var evt    = record["eventName"].ToString();
            var isPut  = "ObjectCreated:Put".Equals(evt);

            Console.WriteLine(evt);

            // Get the source
            var ret = -1;
            var source = service.Source("LLC-Sources", "LLC-Buckets", n, Models.Admin.SourceSearchType.Name);
            //if (source != null)
            //{
                // Look up the object from s3
                var o = service.ObjectGet(n, k);

                // Update the table with the latest S3 count
                var diff = isPut ? 1 : -1;
                ret = service.IncrementMetaTableKey("LLC-Meta", "Objects", diff).Result;
            //}

            return ret;
        }
    }
}
