using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class DecisionOp : Operations
    {
        public static Decision GetDecision(int issueId, int userId)
        {
            return Ctx.Decision.Find(issueId);
        }

        public static List<HDecision> GetOldDecisions(int issueId, int userId)
        {
            return Ctx.HDecision.Where(x => x.IssueId == issueId).OrderByDescending(x => x.ChangeDate).ToList();
        }

        public static void MakeDecision(Decision decision, int userId)
        {
            if (Ctx.Decision.Where(x => x.IssueId == decision.IssueId).Count() == 0)
            {
                Ctx.Decision.Add(decision);
                Ctx.Entry(decision).State = EntityState.Added;
            }else
            {
                Decision existingD = Ctx.Decision.Find(decision.IssueId);
                HDecision dec = new HDecision();
                dec.ChangeDate = DateTime.Now;
                dec.IssueId = decision.IssueId;
                dec.UserId = userId;
                dec.Action = "Decision reconsidered";
                dec.AlternativeId = existingD.AlternativeId;
                dec.Explanation = existingD.Explanation;
                Ctx.HDecision.Add(dec);
                Ctx.Entry(dec).State = EntityState.Added;

                existingD.AlternativeId = decision.AlternativeId;
                existingD.Explanation = decision.Explanation;
                Ctx.Entry(existingD).State = EntityState.Modified;
            }
            Ctx.SaveChanges();
        }

        public static void  UpdateDecision(Decision decision, int userId)
        {
            Decision entity = Ctx.Decision.Where(x => x.IssueId == decision.IssueId).FirstOrDefault();
            entity.Explanation = decision.Explanation;
            Ctx.Entry(entity).State = EntityState.Modified;
            Ctx.SaveChanges();
        }
    }
}
