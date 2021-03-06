//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Db.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class S3Objects
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public S3Objects()
        {
            this.S3Objects_Links = new HashSet<S3Objects_Links>();
        }
    
        public int S3ObjectId { get; set; }
        public int S3BucketId { get; set; }
        public string Key { get; set; }
        public string ItemName { get; set; }
        public string ETag { get; set; }
        public bool IsFolder { get; set; }
        public Nullable<System.DateTime> ContentLastModified { get; set; }
        public System.DateTime DateFirstFound { get; set; }
        public System.DateTime DateLastFound { get; set; }
        public Nullable<System.DateTime> DateLinksLastExtracted { get; set; }
        public Nullable<System.DateTime> LinkCheckDisabledDate { get; set; }
        public string LinkCheckDisabledUser { get; set; }
    
        public virtual S3Buckets S3Buckets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<S3Objects_Links> S3Objects_Links { get; set; }
    }
}
