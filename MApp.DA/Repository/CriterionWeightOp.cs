using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    /// <summary>
    /// class makes operation on Table CriterionWeight
    /// </summary>
    public class CriterionWeightOp
    {
        /// <summary>
        /// gets list of criterionweights which the user should fill out
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId">user who is performing the operation</param>
        /// <returns></returns>
        public static List<CriterionWeight> GetEmptyWeights(int issueId, int userId)
        {
            CriterionWeight cw;
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<CriterionWeight> list = new List<CriterionWeight>();

            var query = from Criterion in ctx.Criterion
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

            foreach (var c in query.AsNoTracking())
            {
                cw = new CriterionWeight();
                cw.CriterionId = c.Id;
                cw.UserId = userId;
                cw.Weight = 0.0;
                list.Add(cw);
            }

            ctx.Dispose();

            return list;
        }

        /// <summary>
        /// returns already weighted criterions of some user for an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<CriterionWeight> GetIssueWeightsOfUser(int issueId, int userId)
        {
            List<CriterionWeight> list = new List<CriterionWeight>();
            CriterionWeight cw;
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            var query = from CriterionWeight in ctx.CriterionWeight
                        where
                              (from Criterion in ctx.Criterion
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

            foreach (var c in query.AsNoTracking())
            {
                cw = new CriterionWeight();
                cw.CriterionId = c.CriterionId;
                cw.Weight = c.Weight;
                cw.UserId = c.UserId;
                list.Add(cw);
            }

            ctx.Dispose();

            return list;
        }

        /// <summary>
        /// returns all criterionweights of an Issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId">user who is performing this operation</param>
        /// <returns></returns>
        public static List<CriterionWeight> GetIssueWeights(int issueId, int userId)
        {
            List<CriterionWeight> list = new List<CriterionWeight>();
            CriterionWeight cw;
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            var query = from CriterionWeight in ctx.CriterionWeight
                        where
                              (from Criterion in ctx.Criterion
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
            foreach (var c in query.AsNoTracking())
            {
                cw = new CriterionWeight();
                cw.CriterionId = c.CriterionId;
                cw.Weight = c.Weight;
                cw.UserId = c.UserId;
                list.Add(cw);
            }

            ctx.Dispose();

            return list;
        }


        /// <summary>
        /// saves user criterion weights into DB
        /// </summary>
        /// <param name="cirteriaWeights"></param>
        /// <param name="issueId"></param>
        /// <param name="userId">user who is performing this operation</param>
        public static void SaveCriterionWeights(List<CriterionWeight> cirteriaWeights, int issueId ,int userId)
        {
            bool insert;
            CriterionWeight updatedCw;
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            //first check if user wants to update his weights
            var query = from CriterionWeight in ctx.CriterionWeight
                        where
                              (from Criterion in ctx.Criterion
                               where
                                Criterion.Issue == issueId &&
                                CriterionWeight.UserId == userId
                               select new
                               {
                                   Criterion.Id
                               }).Contains(new { Id = CriterionWeight.CriterionId })
                        select new
                        {
                            CriterionWeight.Weight
                        };
            if (query.Count() == 0)
            {
                insert = true;
            }else
            {
                insert = false;
            }

            foreach(CriterionWeight cw in cirteriaWeights)
            {
                if (insert)
                {
                    updatedCw = ctx.CriterionWeight.Create();
                    updatedCw.UserId = cw.UserId;
                    updatedCw.CriterionId = cw.CriterionId;
                    updatedCw.Weight = cw.Weight;
                    ctx.CriterionWeight.Add(updatedCw);
                    ctx.Entry(updatedCw).State = EntityState.Added;
                }
                else
                {
                    updatedCw = ctx.CriterionWeight.Find(userId, cw.CriterionId);
                    updatedCw.Weight = cw.Weight;
                    ctx.Entry(updatedCw).State = EntityState.Modified;
                }
                
                ctx.SaveChanges();
            }

            ctx.Dispose();
        }
    }
}
