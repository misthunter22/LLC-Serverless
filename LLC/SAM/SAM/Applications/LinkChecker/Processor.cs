using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using System;
using System.Linq;

namespace SAM.Applications.LinkChecker
{
    public class Processor : BaseHandler
    {
        [LambdaSerializer(typeof(JsonSerializer))]
        public int Handler(object input, ILambdaContext context)
        {
            if (!Service.QueueEmpty())
                return 0;

            var capacity = Service.Settings("DynamoDbThroughput");
            Service.TableCapacity("LLC-Links", int.Parse(capacity.Value), 0);

            var sources = Service.Sources().Where(x => x.AllowLinkChecking);
            var objects = 0;

            foreach (var source in sources)
            {
                Console.WriteLine($"Working on source {source.Id}");
                var objs = Service.LinkChecker(source.Id);
                objects += objs;
            }

            Service.TableCapacity("LLC-Links", 5, 0);
            return objects;
        }
    }
}
