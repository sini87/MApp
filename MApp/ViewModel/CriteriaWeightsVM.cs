using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Web.ViewModel
{
    public class CriteriaWeightsVM
    {
        public IssueModel Issue { get; set; }
        public List<CriterionWeightModel> UserWeights { get; set; }
        public string AccessRight { get; set; }
    }
}