using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class UserIssueModel
    {
        public IssueModel Issue { get; set; }
        public bool CriteriaActionRatingRequired { get; set; }
        public bool SelfAssessmentActionRequired { get; set; }
        public bool EvaluationActionRequired { get; set; }
        public int UnreadCoreItemsCount { get; set; }
        public List<string> UnreadCoreItems { get; set; }
        public double Rating { get; set; }
        public List<ReviewModel> Reviews { get; set; }
    }
}
