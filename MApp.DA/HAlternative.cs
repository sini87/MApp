//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MApp.DA
{
    using System;
    using System.Collections.Generic;
    
    public partial class HAlternative
    {
        public System.DateTime ChangeDate { get; set; }
        public int AlternativeId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public Nullable<double> Rating { get; set; }
        public int IssueId { get; set; }
    
        public virtual Alternative Alternative { get; set; }
        public virtual User User { get; set; }
    }
}
