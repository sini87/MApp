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
    
    public partial class HIssue
    {
        public System.DateTime ChangeDate { get; set; }
        public int IssueId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Setting { get; set; }
        public string AnonymousPosting { get; set; }
        public Nullable<int> Parent { get; set; }
        public Nullable<int> DependsOn { get; set; }
        public string GroupThink { get; set; }
        public Nullable<double> ReviewRating { get; set; }
    
        public virtual Issue Issue { get; set; }
        public virtual User User { get; set; }
    }
}
