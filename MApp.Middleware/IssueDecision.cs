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
    public class IssueDecision
    {
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

        public void SaveDecision(DecisionModel decision, int userId)
        {
            DecisionOp.MakeDecision(decision.ToEntity(decision), userId);
        }

        public void UpdateDecision(DecisionModel decision, int userId)
        {
            DecisionOp.UpdateDecision(decision.ToEntity(decision), userId);
        }
    }
}
