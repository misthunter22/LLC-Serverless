using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM.Models.Dynamo
{
    public class KinesisMetaJob
    {
        public string Table { get; set; }

        public string Key { get; set; }

        public int diff { get; set; }
    }
}
