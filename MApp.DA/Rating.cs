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
    
    public partial class Rating
    {
        public int CriterionId { get; set; }
        public int AlternativeId { get; set; }
        public int UserId { get; set; }
        public double Value { get; set; }
    
        public virtual Alternative Alternative { get; set; }
        public virtual Criterion Criterion { get; set; }
        public virtual User User { get; set; }
    }
}
