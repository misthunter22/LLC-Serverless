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
    
    public partial class LinkStat
    {
        public int LinkStatId { get; set; }
        public int LinkId { get; set; }
        public Nullable<long> ContentSize { get; set; }
        public Nullable<int> DownloadTime { get; set; }
        public System.DateTime DateChecked { get; set; }
        public string ErrorMessage { get; set; }
        public string StatusCode { get; set; }
        public string StatusDesc { get; set; }
        public string ContentType { get; set; }
    
        public virtual Link Link { get; set; }
    }
}