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
            var sources = Service.Sources().Where(x => x.AllowLinkChecking != null && (bool)x.AllowLinkChecking);
            var objects = 0;

            foreach (var source in sources)
            {
                Console.WriteLine($"Working on source {source.Id}");
                //var objs = Service.LinkChecker(source.Id);
                //objects += objs;
            }

            return objects;
        }
    }
}
