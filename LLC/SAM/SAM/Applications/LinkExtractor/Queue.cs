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
            try
            {
                JObject obj = (JObject)input;
                var message = obj["Records"][0]["Sns"]["Message"].ToString();
                Console.WriteLine(message);

                var iteration = Newtonsoft.Json.JsonConvert.DeserializeObject<Iteration>(message);
                Console.WriteLine($"Processing Loop {iteration.Loop} for ID {iteration.Id}");

                var objs = Service.LinkExtractor(iteration.Id, iteration.Loop * MAX, MAX);
                Console.WriteLine($"Iteration {iteration.Loop} count is {objs.Count}");
                Service.EnqueueObjects(objs);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failure, not going to allow retry");
                Console.WriteLine(ex);
            }
        }
    }
}
