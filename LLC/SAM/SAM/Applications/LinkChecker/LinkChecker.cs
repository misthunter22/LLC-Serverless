using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using System;
using System.Linq;

namespace SAM.Applications.LinkChecker
{
    public class LinkChecker : BaseHandler
    {
        [LambdaSerializer(typeof(JsonSerializer))]
        public int Handler(object input, ILambdaContext context)
        {
            if (!Service.QueueEmpty())
                return 0;

            var capacity = Service.Settings("DynamoDbThroughput");
            Service.TableCapacity("LLC-Links", int.Parse(capacity.Value), 0);

            var sources = Service.Sources().Where(x => x.AllowLinkChecking && x.S3BucketId != null);
            var buckets = Service.Buckets();
            var objects = 0;

            foreach (var source in sources)
            {
                var bucket = buckets.FirstOrDefault(x => x.Id == source.S3BucketId);
                if (bucket != null)
                {
                    Console.WriteLine($"Working on bucket {bucket.Id}");
                    var objs = Service.LinkExtractor(bucket.Id);
                    objects += objs;
                }
            }

            Service.TableCapacity("LLC-Links", 5, 0);
            return objects;
        }
    }
}
