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
    
    public partial class Log
    {
        public int LogId { get; set; }
        public System.DateTime DateCreated { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public bool IsError { get; set; }
        public string ExceptionDetails { get; set; }
    }
}
