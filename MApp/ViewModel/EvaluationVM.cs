using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Web.ViewModel
{
    /// <summary>
    /// view model for evaluate alternatives (Evaluation.cshtml)
    /// </summary>
    public class EvaluationVM
    {
        public IssueModel Issue { get; set; }
        public List<RatingModel>[] UserRatings { get; set; }
        public List<CriterionModel> Criterias { get; set; }
        public List<RatingModel> AllRatings { get; set; }
        public List<AlternativeModel> Alternatives { get; set; } 
        public List<UserShortModel> RatedUsers { get; set; }
        public string AccessRight { get; set; }
        public int UserId { get; set; }
        public int RatedUserCnt { get; set; }
        /// <summary>
        /// list of user comparisons
        /// </summary>
        public List<PairwiseComparisonRatingModel> PairwiseRatings { get; set; }
        /// <summary>
        /// pairwise comparison values (eg: equal, ...)
        /// </summary>
        public string[] SliderValues { get; set; }
    }
}