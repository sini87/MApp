using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Web.ViewModel
{
    /// <summary>
    /// view model for decide view (Decision.cshtml)
    /// </summary>
    public class DecisionVM
    {
        public IssueModel Issue { get; set; } 
        public DecisionModel Decision { get; set; }
        public List<AlternativeModel> Alternatives { get; set; }
        public string AccessRight { get; set; }
        public List<DecisionModel> OldDecisions { get; set; }
        public int UserId { get; set; }
    }
}