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
    
    public partial class AccessRight
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AccessRight()
        {
            this.HAccessRight = new HashSet<HAccessRight>();
        }
    
        public int UserId { get; set; }
        public int IssueId { get; set; }
        public string Right { get; set; }
        public Nullable<double> ActivityIndex { get; set; }
        public Nullable<double> SelfAssessmentValue { get; set; }
        public string SelfAssesmentDescr { get; set; }
        public bool MailNotification { get; set; }
        public string NotificationLevel { get; set; }
    
        public virtual Issue Issue { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HAccessRight> HAccessRight { get; set; }
    }
}
