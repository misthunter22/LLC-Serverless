using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using DbCore.Models;
using System.Linq;

namespace SAM.Applications.LinkExtractor
{
    public class LinkExtractor : BaseHandler
    {
        [LambdaSerializer(typeof(JsonSerializer))]
        public void Handler(object input, ILambdaContext context)
        {
            var objs = Service.DequeueObjects<ObjectsExt>();
            if (objs == null)
                return;

            var buckets = Service.Buckets();
            var sources = Service.Sources();

            foreach (var obj in objs)
            {
                // Get the object's file contents
                var bucket  = buckets.FirstOrDefault(x => x.Id == obj.Bucket);
                var source  = sources.FirstOrDefault(x => x.S3bucketId == bucket.Id);

                LinkExtractions(obj.ToObject(), bucket.Name, source.Id);
            }

            Service.RemoveObjectsFromQueue(objs);
        }
    }
}
