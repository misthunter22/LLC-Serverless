using Amazon.Lambda.Core;
using Newtonsoft.Json.Linq;
using System;

namespace SAM.Applications
{
    public class Email : BaseHandler
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public void Handler(object input, ILambdaContext context)
        {
            try
            {
                JObject obj = (JObject)input;
                Console.WriteLine($"Processing {obj["id"].ToString()}");
                Service.SendImpactEmail(obj["id"].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
