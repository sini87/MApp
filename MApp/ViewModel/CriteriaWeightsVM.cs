using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Web.ViewModel
{
    /// <summary>
    /// class used to represend criteria weights of other users
    /// </summary>
    public class UserWithCW
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public List<CriterionWeightModel> UserCriterionWeights { get; set; }
        public UserWithCW()
        {
            
        }

        public UserWithCW(int userId, string name)
        {
            UserId = userId;
            Name = name;
        }
    }

    /// <summary>
    /// view model for criteria weighting (CriteriaRating.cshtml)
    /// </summary>
    public class CriteriaWeightsVM
    {
        public IssueModel Issue { get; set; }
        public List<CriterionWeightModel> UserWeights { get; set; }
        public string AccessRight { get; set; }
        /// <summary>
        /// list of other user weights
        /// </summary>
        public List<CriterionWeightModel>[] OtherWeights { get; set; }
        /// <summary>
        /// list of users who have already voted
        /// </summary>
        public List<UserWithCW> VotedUsers { get; set; }
        public int UserId { get; set; }
        /// <summary>
        /// list of user comparisons
        /// </summary>
        public List<PairwiseComparisonCriterionModel> PCCriteria { get; set; }
        /// <summary>
        /// pairwise comparison values (eg: equal, ...)
        /// </summary>
        public string[] SliderValues { get; set; }
    }
}