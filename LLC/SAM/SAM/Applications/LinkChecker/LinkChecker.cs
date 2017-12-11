using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using DbCore.Models;
using System;
using System.Linq;

namespace SAM.Applications.LinkChecker
{
    public class LinkChecker : BaseHandler
    {
        [LambdaSerializer(typeof(JsonSerializer))]
        public void Handler(object input, ILambdaContext context)
        {
            var objs = Service.DequeueObjects<LinksExt>();
            if (objs == null)
                return;

            foreach (var obj in objs)
            {
                
            }

            Service.RemoveObjectsFromQueue(objs);
        }
    }
}
