using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
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

            foreach (int id in criterionIdList)
            {
                crit = ctx.Criterion.Find(id);
                ctx.Criterion.Remove(crit);
                ctx.Entry(crit).State = EntityState.Deleted;
                ctx.SaveChanges();

                //changes to historytable
                HCriterion hcrit = new HCriterion();
                hcrit.ChangeDate = DateTime.Now;
                hcrit.CriterionId = id;
                hcrit.UserId = userId;
                hcrit.Action = "criterion deleted (" + crit.Name + ")";
                hcrit.Name = crit.Name;
                hcrit.Description = crit.Description;
                hcrit.Issue = crit.Issue;
                hcrit.Weight = crit.Weight;
                hcrit.WeightPC = crit.WeightPC;
                ctx.HCriterion.Add(hcrit);
                ctx.Entry(hcrit).State = EntityState.Added;
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

            foreach (Criterion crit in criterionList)
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

                //changes to history table
                HCriterion hcrit = new HCriterion();
                hcrit.ChangeDate = DateTime.Now;
                hcrit.CriterionId = addingCrit.Id;
                hcrit.UserId = userId;
                hcrit.Action = "criterion added (" + addingCrit.Name + ")";
                hcrit.Name = addingCrit.Name;
                hcrit.Description = addingCrit.Description;
                hcrit.Issue = addingCrit.Issue;
                ctx.HCriterion.Add(hcrit);
                ctx.Entry(hcrit).State = EntityState.Added;
                ctx.SaveChanges();

                //mark new criterion as read
                ApplicationDBEntities ctx2 = new ApplicationDBEntities();
                DbCommand cmd = ctx.Database.Connection.CreateCommand();
                ctx.Database.Connection.Open();
                cmd.CommandText = "UPDATE appSchema.InformationRead SET [Read] = 1 WHERE UserId = " + userId + " AND TName LIKE 'Criterion' AND FK LIKE '" + addingCrit.Id + "'";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();
                ctx2.Database.Connection.Close();
                ctx2.Dispose();
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

                        HCriterion hcrit = new HCriterion();
                        hcrit.ChangeDate = DateTime.Now;
                        hcrit.CriterionId = crit.Id;
                        hcrit.UserId = userId;
                        hcrit.Action = "criterion updated (" + crit.Name + ")";
                        hcrit.Name = crit.Name;
                        hcrit.Description = crit.Description;
                        hcrit.Issue = crit.Issue;
                        hcrit.Weight = crit.Weight;
                        hcrit.WeightPC = crit.WeightPC;
                        ctx.HCriterion.Add(hcrit);
                        ctx.Entry(hcrit).State = EntityState.Added;
                        ctx.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        DbConnection.Instance.DisposeAndReload();
                    }
                }
            }

            ctx.Dispose();
        }
    }
}
