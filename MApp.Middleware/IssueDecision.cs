using MApp.DA;
using MApp.DA.Repository;
using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware
{
    /// <summary>
    /// middleware class for decide view (Decison.cshtml)
    /// </summary>
    public class IssueDecision
    {
        /// <summary>
        /// returns the decision
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user who is performing this operation</param>
        /// <returns>decisonmodel</returns>

        public DecisionModel GetDecision(int issueId, int userId)
        {
            DecisionModel dm = new DecisionModel();
            Decision d = DecisionOp.GetDecision(issueId, userId);
            if (d == null)
            {
                dm.IssueId = issueId;
                return dm;
            }else
            {
                return dm.ToModel(d);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user who is viewing this information</param>
        /// <returns>list of discarded decisions</returns>
        public List<DecisionModel> GetOldDecisions(int issueId, int userId)
        {
            List<HDecision> oldD = DecisionOp.GetOldDecisions(issueId, userId);
            List<DecisionModel> list = new List<DecisionModel>();
            DecisionModel dm;
            foreach (HDecision d in oldD)
            {
                dm = new DecisionModel();
                dm.AlternativeId = Convert.ToInt32(d.AlternativeId);
                dm.ChangeDate = d.ChangeDate;
                dm.Explanation = d.Explanation;
                dm.IssueId = d.IssueId;
                list.Add(dm);
            }

            return list;
        }

        /// <summary>
        /// saves a decision
        /// </summary>
        /// <param name="decision"></param>
        /// <param name="userId"></param>
        public void SaveDecision(DecisionModel decision, int userId)
        {
            DecisionOp.MakeDecision(decision.ToEntity(decision), userId);
        }

        /// <summary>
        /// updates the decision
        /// </summary>
        /// <param name="decision">changed decision</param>
        /// <param name="userId">user who is performing this operation</param>
        public void UpdateDecision(DecisionModel decision, int userId)
        {
            DecisionOp.UpdateDecision(decision.ToEntity(decision), userId);
        }
    }
}
