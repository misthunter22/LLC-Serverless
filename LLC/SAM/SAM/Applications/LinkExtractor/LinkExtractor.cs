using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using DbCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAM.Applications.LinkExtractor
{
    public class LinkExtractor : BaseHandler
    {
        [LambdaSerializer(typeof(JsonSerializer))]
        public void Handler(object input, ILambdaContext context)
        {
            try
            {
                var objs = Service.DequeueObjects<ObjectsExt>();
                if (objs == null)
                    return;

                var buckets = Service.Buckets();
                var sources = Service.Sources();

                foreach (var obj in objs)
                {
                    try
                    {
                        // Get the object's file contents
                        var bucket = buckets.FirstOrDefault(x => x.Id == obj.Bucket);
                        var source = sources.FirstOrDefault(x => x.S3bucketId == bucket.Id);

                        LinkExtractions(obj.ToObject(), bucket.Name, source.Id);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    Service.RemoveObjectsFromQueue(new List<ObjectsExt> { obj });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failure, not going to allow retry");
                Console.WriteLine(ex);
            }
        }
    }
}
