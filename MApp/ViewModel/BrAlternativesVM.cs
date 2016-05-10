using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Web.ViewModel
{
    public class BrAlternativesVM
    {
        public IssueModel Issue { get; set; }
        public List<AlternativeModel> Alternatives { get; set; }
        public List<int> DeletedAlternatives { get; set; }
        public string AccessRight { get; set; }
    }
}