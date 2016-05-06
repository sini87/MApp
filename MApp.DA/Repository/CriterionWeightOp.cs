using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class CriterionWeightOp : Operations
    {

        public static List<CriterionWeight> GetEmptyWeights(int issueId, int userId)
        {
            CriterionWeight cw;
            List<CriterionWeight> list = new List<CriterionWeight>();

            var query = from Criterion in Ctx.Criterion
                        where
                          Criterion.Issue == issueId
                        select new
                        {
                            Id = Criterion.Id,
                            Name = Criterion.Name,
                            Description = Criterion.Description,
                            Issue = Criterion.Issue,
                            Weight = Criterion.Weight,
                            WeightPC = Criterion.WeightPC
                        };

            foreach (var c in query)
            {
                cw = new CriterionWeight();
                cw.CriterionId = c.Id;
                cw.UserId = userId;
                cw.Weight = 0.0;
                list.Add(cw);
            }
            return list;
        }

        public static List<CriterionWeight> GetIssueWeightsOfUser(int issueId, int userId)
        {
            List<CriterionWeight> list = new List<CriterionWeight>();
            CriterionWeight cw;
            var query = from CriterionWeight in Ctx.CriterionWeight
                        where
                              (from Criterion in Ctx.Criterion
                               where
                               Criterion.Issue == issueId
                               select new
                               {
                                   Criterion.Id
                               }).Contains(new { Id = CriterionWeight.CriterionId }) &&
                          CriterionWeight.UserId == userId
                        select new
                        {
                            UserId = CriterionWeight.UserId,
                            CriterionId = CriterionWeight.CriterionId,
                            Weight = CriterionWeight.Weight
                        };

            foreach (var c in query)
            {
                cw = new CriterionWeight();
                cw.CriterionId = c.CriterionId;
                cw.Weight = c.Weight;
                cw.UserId = c.UserId;
                list.Add(cw);
            }
            return list;
        }

        public static List<CriterionWeight> GetIssueWeights(int issueId, int userId)
        {
            List<CriterionWeight> list = new List<CriterionWeight>();
            CriterionWeight cw;
            var query = from CriterionWeight in Ctx.CriterionWeight
                        where
                              (from Criterion in Ctx.Criterion
                               where
                               Criterion.Issue == issueId
                               select new
                               {
                                   Criterion.Id
                               }).Contains(new { Id = CriterionWeight.CriterionId }) 
                        select new
                        {
                            UserId = CriterionWeight.UserId,
                            CriterionId = CriterionWeight.CriterionId,
                            Weight = CriterionWeight.Weight
                        };
            foreach (var c in query)
            {
                cw = new CriterionWeight();
                cw.CriterionId = c.CriterionId;
                cw.Weight = c.Weight;
                cw.UserId = c.UserId;
                list.Add(cw);
            }
            return list;
        }
    }
}
