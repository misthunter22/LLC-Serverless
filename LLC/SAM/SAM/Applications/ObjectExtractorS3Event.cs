﻿using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAM.DI;
using System;

namespace SAM.Applications
{
    public class ObjectExtractorS3Event : BaseHandler
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public void Handler(object input, ILambdaContext context)
        {
            Console.WriteLine(JsonConvert.SerializeObject(input));
            Console.WriteLine(input.GetType());
            JObject obj = (JObject)input;

            var evt  = obj["Records"][0]["eventName"].ToString();
            Console.WriteLine(evt);
            var diff = "ObjectCreated:Put".Equals(evt) ? 1 : -1;

            var service = new ILLCDataImpl();
            service.SubmitMetaTableQueue("LLC-Meta", "Test", diff);
        }
    }
}
