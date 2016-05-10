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
    public class CriterionOp : Operations
    {
        /// <summary>
        /// returns all Criteria of an Issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId">user who is performing this operation</param>
        /// <returns></returns>
        public static List<Criterion> GetIssueCriterions(int issueId, int userId)
        {
            List<Criterion> list = Ctx.Criterion.Where(x => x.Issue == issueId).ToList();
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

            if (criterionIdList == null || criterionIdList.Count == 0)
            {
                return;
            }

            foreach(int id in criterionIdList)
            {
                crit = Ctx.Criterion.Find(id);
                Ctx.Criterion.Remove(crit);
                Ctx.Entry(crit).State = EntityState.Deleted;
                Ctx.SaveChanges();
            }
        }

        /// <summary>
        /// adds a list of criteria to an issue
        /// </summary>
        /// <param name="criterionList">list of criteria</param>
        /// <param name="userId">user who is performing this operation</param>
        public static void AddCriterions(List<Criterion> criterionList, int userId)
        {
            Criterion addingCrit;

            if(criterionList == null || criterionList.Count == 0)
            {
                return;
            }

            foreach(Criterion crit in criterionList)
            {
                addingCrit = Ctx.Criterion.Create();
                addingCrit.Description = crit.Description;
                addingCrit.Name = crit.Name;
                addingCrit.Weight = null;
                addingCrit.WeightPC = null;
                addingCrit.Issue = crit.Issue;
                Ctx.Criterion.Add(addingCrit);
                Ctx.Entry(addingCrit).State = EntityState.Added;
                Ctx.SaveChanges();
            }
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

            if (criterionList == null || criterionList.Count == 0)
            {
                return;
            }

            foreach (Criterion crit in criterionList)
            {
                updated = false;
                updatingCrit = Ctx.Criterion.Find(crit.Id);
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
                    Ctx.Entry(updatingCrit).State = EntityState.Modified;
                    try
                    {
                        Ctx.SaveChanges();
                    }catch(Exception ex)
                    {
                        DbConnection.Instance.DisposeAndReload();
                    }
                    
                }
            }
        }
    }
}
