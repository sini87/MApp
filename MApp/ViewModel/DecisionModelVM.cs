using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MApp.Web.ViewModel
{
    /// <summary>
    /// model for decide view
    /// </summary>
    public class DecisionModelVM
    {
        public int IssueId { get; set; }
        public int AlternativeId { get; set; }
        [AllowHtml]
        public string Explanation { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}