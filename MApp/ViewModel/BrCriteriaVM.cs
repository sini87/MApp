using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Web.ViewModel
{
    /// <summary>
    /// View Model for prepare criteira (BrCriteria.cshtml)
    /// </summary>
    public class BrCriteriaVM
    {
        public IssueModel Issue { get; set; }
        public List<CriterionModel> IssueCriteria { get; set; }
        /// <summary>
        /// list of criteria ids which have to be deleted
        /// used for post
        /// </summary>
        public List<int> DeletedCriteria { get; set; }
        /// <summary>
        /// access right of user who is opening view
        /// </summary>
        public string AccessRight { get; set; }
        public int UserId { get; set; }
    }
}