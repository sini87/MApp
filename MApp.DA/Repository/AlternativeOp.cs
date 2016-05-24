using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    /// <summary>
    /// Class represents all operations to Table Alternative
    /// </summary>
    public class AlternativeOp
    {
        /// <summary>
        /// returns alternatives of issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId">user who is performing this operation</param>
        /// <returns></returns>
        public static List<Alternative> GetIssueAlternatives(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<Alternative> list = ctx.Alternative.AsNoTracking().Where(x => x.IssueId == issueId).ToList();
            return list;
        }

        /// <summary>
        /// deletes list of alternatives
        /// </summary>
        /// <param name="alternativeIdList">list of to deleting alternative ids</param>
        /// <param name="userId"></param>
        public static void DeleteAlternatives(List<int> alternativeIdList, int userId)
        {
            Alternative alt;
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            if (alternativeIdList == null || alternativeIdList.Count() == 0)
            {
                return;
            }

            foreach (int id in alternativeIdList)
            {
                alt = ctx.Alternative.Find(id);
                ctx.Alternative.Remove(alt);
                ctx.Entry(alt).State = EntityState.Deleted;
                ctx.SaveChanges();

                HAlternative halt = new HAlternative();
                halt.ChangeDate = DateTime.Now;
                halt.AlternativeId = alt.Id;
                halt.UserId = userId;
                halt.Action = "alternative deleted (" + alt.Name + ")";
                halt.Name = alt.Name;
                halt.Description = alt.Description;
                halt.Reason = alt.Reason;
                halt.Rating = alt.Rating;
                halt.IssueId = alt.IssueId;
                ctx.HAlternative.Add(halt);
                ctx.Entry(halt).State = EntityState.Added;
                ctx.SaveChanges();
            }

            ctx.Dispose();
            CommentOp.DeleteAlternativeComments(alternativeIdList);
        }

        /// <summary>
        /// adds a List of Alternatives to an issue
        /// </summary>
        /// <param name="alternativeList"></param>
        /// <param name="userId">user who is performing this operation</param>
        public static void AddAlternatives(List<Alternative> alternativeList, int userId)
        {
            Alternative addedAlt;
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            if (alternativeList == null || alternativeList.Count() == 0)
            {
                return;
            }

            foreach (Alternative alt in alternativeList)
            {
                addedAlt = ctx.Alternative.Create();
                addedAlt.Description = alt.Description;
                addedAlt.IssueId = alt.IssueId;
                addedAlt.Name = alt.Name;
                addedAlt.Reason = alt.Reason;
                ctx.Alternative.Add(addedAlt);
                ctx.Entry(addedAlt).State = EntityState.Added;
                ctx.SaveChanges();

                //insert into change-table
                HAlternative halt = new HAlternative();
                halt.ChangeDate = DateTime.Now;
                halt.AlternativeId = addedAlt.Id;
                halt.UserId = userId;
                halt.IssueId = alt.IssueId;
                halt.Action = "alternative added (" + addedAlt.Name + ")";
                halt.Name = addedAlt.Name;
                halt.Description = addedAlt.Description;
                halt.Reason = addedAlt.Reason;
                ctx.HAlternative.Add(halt);
                ctx.Entry(halt).State = EntityState.Added;
                ctx.SaveChanges();

                //mark new alternative as read
                ApplicationDBEntities ctx2 = new ApplicationDBEntities();
                DbCommand cmd = ctx.Database.Connection.CreateCommand();
                ctx.Database.Connection.Open();
                cmd.CommandText = "UPDATE appSchema.InformationRead SET [Read] = 1 WHERE UserId = " + userId + " AND TName LIKE 'Alternative' AND FK LIKE '" + addedAlt.Id + "'";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();
                ctx2.Database.Connection.Close();
                ctx2.Dispose();
            }

            ctx.Dispose();
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
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            if (alternativeList == null || alternativeList.Count() == 0)
            {
                return;
            }

            foreach (Alternative alt in alternativeList)
            {
                updated = false;
                updateAlt = ctx.Alternative.Find(alt.Id);
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
                    ctx.Entry(updateAlt).State = EntityState.Modified;
                    ctx.SaveChanges();

                    HAlternative halt = new HAlternative();
                    halt.ChangeDate = DateTime.Now;
                    halt.AlternativeId = updateAlt.Id;
                    halt.UserId = useId;
                    halt.Action = "alternative updated (" + updateAlt.Name + ")";
                    halt.Name = updateAlt.Name;
                    halt.Description = updateAlt.Description;
                    halt.Reason = updateAlt.Reason;
                    halt.Rating = updateAlt.Rating;
                    halt.IssueId = updateAlt.IssueId;
                    ctx.HAlternative.Add(halt);
                    ctx.Entry(halt).State = EntityState.Added;
                    ctx.SaveChanges();
                }
            }

            ctx.Dispose();
        }
    }
}
