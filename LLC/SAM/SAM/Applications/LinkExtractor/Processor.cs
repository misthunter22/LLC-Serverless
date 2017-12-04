using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using System;
using System.Linq;

namespace SAM.Applications.LinkExtractor
{
    public class Processor : BaseHandler
    {
        [LambdaSerializer(typeof(JsonSerializer))]
        public int Handler(object input, ILambdaContext context)
        {
            var sources = Service.Sources().Where(x => x.AllowLinkExtractions != null && (bool)x.AllowLinkChecking && x.S3bucketId != null);
            var buckets = Service.Buckets();
            var objects = 0;

            foreach (var source in sources)
            {
                var bucket = buckets.FirstOrDefault(x => x.Id == source.S3bucketId);
                if (bucket != null)
                {
                    //Console.WriteLine($"Working on bucket {bucket.Id}");
                    //var objs = Service.LinkExtractor(bucket.Id);
                    //objects += objs;
                }
            }

            return objects;
        }
    }
}