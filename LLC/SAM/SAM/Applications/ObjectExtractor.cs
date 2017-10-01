using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using SAM.DI;
using System;

namespace SAM.Applications
{
    public class ObjectExtractor : BaseHandler
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public int Handler(object input, ILambdaContext context)
        {
            var service = new ILLCDataImpl();
            using (var client = new AmazonDynamoDBClient(service.Region()))
            {
                using (var ctx = new DynamoDBContext(client))
                {
                    // http://docs.amazonaws.cn/en_us/amazondynamodb/latest/developerguide/DynamoDBContext.VersionSupport.html
                    Console.WriteLine(JsonConvert.SerializeObject(input));
                    return 5;
                }
            }
        }
    }
}
