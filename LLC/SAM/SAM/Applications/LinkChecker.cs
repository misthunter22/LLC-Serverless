﻿using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM.Applications
{
    public class LinkChecker
    {
        [LambdaSerializer(typeof(JsonSerializer))]
        public int Handler(object input, ILambdaContext context)
        {
            return 3;
        }
    }
}
