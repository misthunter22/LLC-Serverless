using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM.Models.Dynamo
{
    public abstract class ReceiptBase
    {
        public string ReceiptHandle { get; set; }
    }
}
