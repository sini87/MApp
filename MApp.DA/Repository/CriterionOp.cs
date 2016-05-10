using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    /// <summary>
    /// In this are class all functions regarding table Criteria
    /// </summary>
    public class CriterionOp
    {
        /// <summary>
        /// returns all Criteria of an Issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId">user who is performing this operation</param>
        /// <returns></returns>
        public static List<Criterion> GetIssueCriterions(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<Criterion> list = ctx.Criterion.AsNoTracking().Where(x => x.Issue == issueId).ToList();
            ctx.Dispose();
            return list;
        }

        /// <summary>
        /// deletes criterions by criterionId
        /// </summary>
        /// <param name="criterionIdList">list of criteria</param>
        /// <param name="userId">user who is performing this operation</param>
        public static void DeleteCriterions(List<int> criterionIdList, int userId)
        {
            Criterion crit;
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            if (criterionIdList == null || criterionIdList.Count == 0)
            {
                return;
            }

            foreach(int id in criterionIdList)
            {
                crit = ctx.Criterion.Find(id);
                ctx.Criterion.Remove(crit);
                ctx.Entry(crit).State = EntityState.Deleted;
                ctx.SaveChanges();
            }

            ctx.Dispose();
        }

        /// <summary>
        /// adds a list of criteria to an issue
        /// </summary>
        /// <param name="criterionList">list of criteria</param>
        /// <param name="userId">user who is performing this operation</param>
        public static void AddCriterions(List<Criterion> criterionList, int userId)
        {
            Criterion addingCrit;
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            if (criterionList == null || criterionList.Count == 0)
            {
                return;
            }

            foreach(Criterion crit in criterionList)
            {
                addingCrit = ctx.Criterion.Create();
                addingCrit.Description = crit.Description;
                addingCrit.Name = crit.Name;
                addingCrit.Weight = null;
                addingCrit.WeightPC = null;
                addingCrit.Issue = crit.Issue;
                ctx.Criterion.Add(addingCrit);
                ctx.Entry(addingCrit).State = EntityState.Added;
                ctx.SaveChanges();
            }

            ctx.Dispose();
        }

        /// <summary>
        /// updates a lsit of criteria
        /// </summary>
        /// <param name="criterionList">list ofr criteria</param>
        /// <param name="userId">user who is performing this operation</param>
        public static void UpdateCriterions(List<Criterion> criterionList, int userId)
        {
            bool updated;
            Criterion updatingCrit;
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            if (criterionList == null || criterionList.Count == 0)
            {
                return;
            }

            foreach (Criterion crit in criterionList)
            {
                updated = false;
                updatingCrit = ctx.Criterion.Find(crit.Id);
                if (updatingCrit.Name != crit.Name)
                {
                    updated = true;
                    updatingCrit.Name = crit.Name;
                }
                if (updatingCrit.Description != crit.Description)
                {
                    updated = true;
                    updatingCrit.Description = crit.Description;
                }
                if (updated)
                {
                    ctx.Entry(updatingCrit).State = EntityState.Modified;
                    try
                    {
                        ctx.SaveChanges();
                    }catch(Exception ex)
                    {
                        DbConnection.Instance.DisposeAndReload();
                    }
                }
            }

            ctx.Dispose();
        }
    }
}
