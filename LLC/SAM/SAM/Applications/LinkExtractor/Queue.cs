using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Newtonsoft.Json.Linq;
using System;

namespace SAM.Applications.LinkExtractor
{
    public class Queue : BaseHandler
    {
        [LambdaSerializer(typeof(JsonSerializer))]
        public void Handler(object input, ILambdaContext context)
        {
            JObject obj   = (JObject)input;

            var message   = obj["Records"][0]["Sns"]["Message"].ToString();
            Console.WriteLine(message);

            var iteration = Newtonsoft.Json.JsonConvert.DeserializeObject<Iteration>(message);
            Console.WriteLine($"Processing Loop {iteration.Loop} for Bucket {iteration.Bucket}");

            var objs = Service.LinkExtractor(iteration.Bucket, iteration.Loop * MAX, MAX);
            Console.WriteLine($"Iteration {iteration.Loop} object count is {objs.Count}");
            Service.EnqueueObjects(objs);
        }
    }
}
