﻿using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Web.ViewModel
{
    /// <summary>
    /// View Model for BrCriteria
    /// </summary>
    public class BrCriteriaVM
    {
        public IssueModel Issue { get; set; }
        public List<CriterionModel> IssueCriteria { get; set; }
        public List<int> DeletedCriteria { get; set; }
        public string AccessRight { get; set; }
        public int UserId { get; set; }
    }
}