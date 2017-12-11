using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Linq;

namespace SAM.Applications.LinkExtractor
{
    public class Processor : BaseHandler
    {
        [LambdaSerializer(typeof(JsonSerializer))]
        public int Handler(object input, ILambdaContext context)
        {
            if (!Service.QueueEmpty())
                return 0;

            var sources = Service.Sources().Where(x => x.AllowLinkExtractions != null && (bool)x.AllowLinkChecking && x.S3bucketId != null);
            var buckets = Service.Buckets();
            var objects = 0;
            var topic   = Environment.GetEnvironmentVariable("Topic");
            Console.WriteLine($"Publishing to topic {topic}");

            foreach (var source in sources)
            {
                var bucket = buckets.FirstOrDefault(x => x.Id == source.S3bucketId);
                if (bucket != null)
                {
                    Console.WriteLine($"Working on bucket {bucket.Id}");
                    var count = Service.ObjectsCount(bucket.Id);
                    Console.WriteLine($"Object count is {count}");
                    var loop = count < MAX ? 1 : Math.Ceiling(((double)count) / ((double)MAX));
                    Console.WriteLine($"Loop count is {loop}");
                    for (var i = 0; i < loop; i++)
                    {
                        using (var sns = new AmazonSimpleNotificationServiceClient())
                        {
                            var message = Newtonsoft.Json.JsonConvert.SerializeObject(new Iteration
                            {
                                Bucket = bucket.Id,
                                Loop = i
                            });

                            Console.WriteLine($"Message to publish is {message}");
                            var result = sns.PublishAsync(new PublishRequest
                            {
                                Subject = $"Link Extractor Iteration {i}",
                                TopicArn = topic,
                                Message = message
                            }).Result;
                        }
                    }
                }
            }

            return objects;
        }
    }
}