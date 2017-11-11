using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using SAM.Models.Dynamo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAM.Applications.LinkExtractor
{
    public class Count : BaseHandler
    {
        [LambdaSerializer(typeof(JsonSerializer))]
        public int Handler(object input, ILambdaContext context)
        {
            var sources  = Service.Sources().Where(x => x.AllowLinkChecking && x.S3BucketId != null);
            var buckets  = Service.Buckets();
            var objects  = new List<Objects>();

            foreach (var source in sources) {
                var bucket = buckets.FirstOrDefault(x => x.Id == source.S3BucketId);
                if (bucket != null)
                {
                    /*Console.WriteLine($"Working on bucket {bucket.Id}");
                    var objs = Service.LinkExtractor(bucket.Id);*/
                    Console.WriteLine($"Working on bucket 1");
                    var objs = Service.LinkExtractor("1");
                    objects.AddRange(objs);
                    Console.WriteLine($"{bucket.Name} count is {objs.Count}");
                }
            }

            var s3Bucket = Environment.GetEnvironmentVariable("AppS3Bucket");
            Service.ObjectPut(s3Bucket, "LinkExtractor/objects.json", objects);

            return objects.Count;
        }
    }
}
