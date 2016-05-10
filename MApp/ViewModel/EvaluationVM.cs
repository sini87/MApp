using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Web.ViewModel
{
    public class EvaluationVM
    {
        public IssueModel Issue { get; set; }
        public List<RatingModel>[] UserRatings { get; set; }
        public List<CriterionModel> Criterias { get; set; }
        public List<RatingModel> AllRatings { get; set; }
        public List<AlternativeModel> Alternatives { get; set; } 
        public List<UserShortModel> RatedUsers { get; set; }
        public string AccessRight { get; set; }
    }
}