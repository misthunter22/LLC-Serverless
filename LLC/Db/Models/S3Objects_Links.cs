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
    
    public partial class S3Objects_Links
    {
        public int S3ObjectLinkId { get; set; }
        public int S3ObjectId { get; set; }
        public int LinkId { get; set; }
        public System.DateTime DateFirstFound { get; set; }
        public System.DateTime DateLastFound { get; set; }
        public Nullable<System.DateTime> DateRemoved { get; set; }
    
        public virtual Link Link { get; set; }
        public virtual S3Objects S3Objects { get; set; }
    }
}
