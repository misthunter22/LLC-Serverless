using System;
using System.Collections.Generic;

namespace DbCore.Models
{
    public partial class Objects
    {
        public string Id { get; set; }
        public string Bucket { get; set; }
        public string Key { get; set; }
        public string ItemName { get; set; }
        public string Etag { get; set; }
        public bool? IsFolder { get; set; }
        public DateTime? ContentLastModified { get; set; }
        public DateTime? DateFirstFound { get; set; }
        public DateTime? DateLastFound { get; set; }
        public DateTime? DateLinksLastExtracted { get; set; }
        public DateTime? DisabledDate { get; set; }
        public string DisabledUser { get; set; }
    }
}
