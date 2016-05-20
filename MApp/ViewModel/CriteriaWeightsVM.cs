using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Web.ViewModel
{
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
    public class CriteriaWeightsVM
    {
        public IssueModel Issue { get; set; }
        public List<CriterionWeightModel> UserWeights { get; set; }
        public string AccessRight { get; set; }
        public List<CriterionWeightModel>[] OtherWeights { get; set; }
        public List<UserWithCW> VotedUsers { get; set; }
        public int UserId { get; set; }
    }
}