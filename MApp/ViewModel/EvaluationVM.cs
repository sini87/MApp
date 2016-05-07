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
        public List<RatingModel> Ratings { get; set; }
        public List<CriterionModel> Criterias { get; set; }
        public List<RatingModel> AllRatings { get; set; }
    }
}