using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Web.ViewModel
{
    /// <summary>
    /// viewmodel for prepare alternatives view (BrAlternatives.cshtml)
    /// </summary>
    public class BrAlternativesVM
    {
        public IssueModel Issue { get; set; }
        public List<AlternativeModel> Alternatives { get; set; }
        /// <summary>
        /// list of deleting alternative ids
        /// used for posting
        /// </summary>
        public List<int> DeletedAlternatives { get; set; }
        /// <summary>
        /// access right of user who is opening view
        /// </summary>
        public string AccessRight { get; set; }
        public int UserId { get; set; }
    }
}