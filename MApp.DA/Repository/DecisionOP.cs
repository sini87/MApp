using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class DecisionOp
    {
        /// <summary>
        /// gets decision for an Issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId">user who is performing this operation</param>
        /// <returns></returns>
        public static Decision GetDecision(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            Decision decision = ctx.Decision.AsNoTracking().Where(x => x.IssueId == issueId).FirstOrDefault();
            ctx.Dispose();

            return decision;
        }

        /// <summary>
        /// gets previous decisions of an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId">user who is performing this operation</param>
        /// <returns></returns>
        public static List<HDecision> GetOldDecisions(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<HDecision> rlist = new List<HDecision>();

            List<HDecision> list = ctx.HDecision.AsNoTracking().Where(x => x.IssueId == issueId).OrderByDescending(x => x.ChangeDate).ToList();
            if (list.Count > 1)
            {
                HDecision youngest = list[0];
                for (int i = list.Count - 1; i > 0; i--)
                {
                    list[i].ChangeDate = list[i - 1].ChangeDate;
                }
                list.Remove(youngest);
                rlist = list;
            }
            ctx.Dispose();

            return rlist;
        }

        /// <summary>
        /// sets a decision for an issue
        /// </summary>
        /// <param name="decision">the decision</param>
        /// <param name="userId">user who is performing this operation</param>
        public static void MakeDecision(Decision decision, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            HDecision dec = new HDecision();
            if (ctx.Decision.Where(x => x.IssueId == decision.IssueId).Count() == 0)
            {
                ctx.Decision.Add(decision);
                ctx.Entry(decision).State = EntityState.Added;
                dec.Action = "Decision made";
            }
            else
            {
                Decision existingD = ctx.Decision.Find(decision.IssueId);

                existingD.AlternativeId = decision.AlternativeId;
                existingD.Explanation = decision.Explanation;
                ctx.Entry(existingD).State = EntityState.Modified;
                dec.Action = "Decision changed";
            }

            dec.ChangeDate = DateTime.Now;
            dec.IssueId = decision.IssueId;
            dec.UserId = userId;
            dec.AlternativeId = decision.AlternativeId;
            dec.Explanation = decision.Explanation;
            ctx.HDecision.Add(dec);
            ctx.Entry(dec).State = EntityState.Added;

            ctx.SaveChanges();
            ctx.Dispose();
        }

        /// <summary>
        /// overthinks an decision
        /// </summary>
        /// <param name="decision"></param>
        /// <param name="userId"></param>
        public static void  UpdateDecision(Decision decision, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            Decision entity = ctx.Decision.Where(x => x.IssueId == decision.IssueId).FirstOrDefault();
            entity.Explanation = decision.Explanation;
            ctx.Entry(entity).State = EntityState.Modified;
            ctx.SaveChanges();

            ctx.Dispose();
        }
    }
}
