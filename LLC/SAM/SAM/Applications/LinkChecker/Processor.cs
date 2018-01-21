using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Linq;

namespace SAM.Applications.LinkChecker
{
    public class Processor : BaseHandler
    {
        [LambdaSerializer(typeof(JsonSerializer))]
        public int Handler(object input, ILambdaContext context)
        {
            try
            {
                if (!Service.QueueEmpty())
                    return 0;

                var topic = Environment.GetEnvironmentVariable("Topic");
                var sources = Service.Sources().Where(x => x.AllowLinkChecking != null && (bool)x.AllowLinkChecking);
                var objects = 0;

                foreach (var source in sources)
                {
                    Console.WriteLine($"Working on source {source.Id}");
                    var count = Service.LinksCount(source.Id);
                    Console.WriteLine($"Object count is {count}");
                    var loop = count < MAX ? 1 : Math.Ceiling(((double)count) / ((double)MAX));
                    Console.WriteLine($"Loop count is {loop}");
                    for (var i = 0; i < loop; i++)
                    {
                        using (var sns = new AmazonSimpleNotificationServiceClient())
                        {
                            var message = Newtonsoft.Json.JsonConvert.SerializeObject(new Iteration
                            {
                                Id = source.Id,
                                Loop = i
                            });

                            Console.WriteLine($"Message to publish is {message}");
                            var result = sns.PublishAsync(new PublishRequest
                            {
                                Subject = $"Link Checker Iteration {i}",
                                TopicArn = topic,
                                Message = message
                            }).Result;
                        }
                    }
                }

                return objects;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failure, not going to allow retry");
                Console.WriteLine(ex);
                return -1;
            }
        }
    }
}
