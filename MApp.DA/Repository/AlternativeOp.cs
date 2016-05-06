using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    /// <summary>
    /// Class represents all operations to Table Alternative
    /// </summary>
    public class AlternativeOp : Operations
    {
        /// <summary>
        /// returns alternatives of issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId">user who is performing this operation</param>
        /// <returns></returns>
        public static List<Alternative> GetIssueAlternatives(int issueId, int userId)
        {
            return Ctx.Alternative.Where(x => x.IssueId == issueId).ToList();
        }

        /// <summary>
        /// deletes list of alternatives
        /// </summary>
        /// <param name="alternativeIdList">list of to deleting alternative ids</param>
        /// <param name="userId"></param>
        public static void DeleteAlternatives (List<int> alternativeIdList, int userId)
        {
            Alternative alt;
            if (alternativeIdList == null || alternativeIdList.Count() == 0)
            {
                return;
            }

            foreach (int id in alternativeIdList)
            {
                alt = Ctx.Alternative.Find(id);
                Ctx.Alternative.Remove(alt);
                Ctx.Entry(alt).State = EntityState.Deleted;
                Ctx.SaveChanges();
            }
        }

        /// <summary>
        /// adds a List of Alternatives to an issue
        /// </summary>
        /// <param name="alternativeList"></param>
        /// <param name="userId">user who is performing this operation</param>
        public static void AddAlternatives (List<Alternative> alternativeList, int userId)
        {
            Alternative addedAlt;
            if (alternativeList == null || alternativeList.Count() == 0)
            {
                return;
            }

            foreach (Alternative alt in alternativeList)
            {
                addedAlt = Ctx.Alternative.Create();
                addedAlt.Description = alt.Description;
                addedAlt.IssueId = alt.IssueId;
                addedAlt.Name = alt.Name;
                addedAlt.Reason = alt.Reason;
                Ctx.Alternative.Add(addedAlt);
                Ctx.Entry(addedAlt).State = EntityState.Added;
                Ctx.SaveChanges();
            }
        }

        /// <summary>
        /// updates a list of alternatives
        /// </summary>
        /// <param name="alternativeList"></param>
        /// <param name="useId">user who is performing this operation</param>
        public static void UpdateAlternatives(List<Alternative> alternativeList, int useId)
        {
            Alternative updateAlt;
            bool updated;

            if (alternativeList == null || alternativeList.Count() == 0)
            {
                return;
            }

            foreach (Alternative alt in alternativeList)
            {
                updated = false;
                updateAlt = Ctx.Alternative.Find(alt.Id);
                if (alt.Description != updateAlt.Description || !alt.Description.Equals(updateAlt.Description))
                {
                    updateAlt.Description = alt.Description;
                    updated = true;
                }
                if (alt.Name != updateAlt.Name || !alt.Name.Equals(updateAlt.Name))
                {
                    updateAlt.Name = alt.Name;
                    updated = true;
                }
                if (!(alt.Reason == null && updateAlt.Reason == null))
                {
                    if (alt.Reason != updateAlt.Reason || !alt.Reason.Equals(updateAlt.Reason))
                    {
                        updateAlt.Reason = alt.Reason;
                        updated = true;
                    }
                }
                    
                if (updated)
                {
                    Ctx.Entry(updateAlt).State = EntityState.Modified;
                    Ctx.SaveChanges();
                }
            }
        }
    }
}
