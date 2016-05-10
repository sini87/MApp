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
            List<HDecision> list = ctx.HDecision.AsNoTracking().Where(x => x.IssueId == issueId).OrderByDescending(x => x.ChangeDate).ToList();
            ctx.Dispose();

            return list;
        }

        /// <summary>
        /// sets a decision for an issue
        /// </summary>
        /// <param name="decision">the decision</param>
        /// <param name="userId">user who is performing this operation</param>
        public static void MakeDecision(Decision decision, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            if (ctx.Decision.Where(x => x.IssueId == decision.IssueId).Count() == 0)
            {
                ctx.Decision.Add(decision);
                ctx.Entry(decision).State = EntityState.Added;
            }else
            {
                Decision existingD = ctx.Decision.Find(decision.IssueId);
                HDecision dec = new HDecision();
                dec.ChangeDate = DateTime.Now;
                dec.IssueId = decision.IssueId;
                dec.UserId = userId;
                dec.Action = "Decision reconsidered";
                dec.AlternativeId = existingD.AlternativeId;
                dec.Explanation = existingD.Explanation;
                ctx.HDecision.Add(dec);
                ctx.Entry(dec).State = EntityState.Added;

                existingD.AlternativeId = decision.AlternativeId;
                existingD.Explanation = decision.Explanation;
                ctx.Entry(existingD).State = EntityState.Modified;
            }
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
