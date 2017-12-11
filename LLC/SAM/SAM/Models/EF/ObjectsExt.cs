using SAM.Models.Dynamo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbCore.Models
{
    public class ObjectsExt : ReceiptBase
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

        public Objects ToObject()
        {
            return new Objects
            {
                Bucket = Bucket,
                ContentLastModified = ContentLastModified,
                DateFirstFound = DateFirstFound,
                DateLastFound = DateLastFound,
                DateLinksLastExtracted = DateLinksLastExtracted,
                DisabledDate = DisabledDate,
                DisabledUser = DisabledUser,
                Etag = Etag,
                Id = Id,
                IsFolder = IsFolder,
                ItemName = ItemName,
                Key = Key
            };
        }
    }
}
