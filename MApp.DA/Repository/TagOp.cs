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
    /// makes operation on table Tag
    /// </summary>
    public class TagOp
    {
        /// <summary>
        /// returns all available Tags
        /// </summary>
        /// <returns></returns>
        public static List<Tag> GetAllTags()
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<Tag> list = ctx.Tag.AsNoTracking().ToList();
            ctx.Dispose();
            return list;
        }

        /// <summary>
        /// returns all tags of Issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public static List<Tag> GetIssueTags(int issueId)
        {
            List<Tag> list = new List<Tag>();
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            foreach (TagIssue ti in ctx.TagIssue.AsNoTracking().Where(ti => ti.IssueId == issueId).ToList())
            {
                list.Add(ti.Tag);
            }

            ctx.Dispose();

            return list;
        }

        /// <summary>
        /// adds tags to issue
        /// </summary>
        /// <param name="tagList">list of tags (if tag id is -1 then new tag will be created)</param>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user who is performing operation</param>
        public static void AddTagsToIssue(List<Tag> tagList, int issueId, int userId)
        {
            string sql;
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            ApplicationDBEntities ctx2 = new ApplicationDBEntities();
            DbCommand cmd;
            ctx2.Database.Connection.Open();
            foreach (Tag tag in tagList)
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    if (tag.Id == -1)
                    {
                        sql = "INSERT INTO appSchema.[Tag] (Name) OUTPUT INSERTED.Id VALUES ({0})";
                        var res = ctx.Database.SqlQuery<int>(sql, tag.Name);
                        dbContextTransaction.Commit();
                        tag.Id = res.FirstOrDefault();
                    }
                    sql = "INSERT INTO appSchema.[TagIssue] VALUES (" + tag.Id + "," + issueId + ")";
                    cmd = ctx2.Database.Connection.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.CommandType = System.Data.CommandType.Text;
                    
                    try
                    {
                        cmd.ExecuteNonQuery();

                        //changes to history table
                        HTagIssue htagIssue = new HTagIssue();
                        htagIssue.ChangeDate = DateTime.Now;
                        htagIssue.TagId = tag.Id;
                        htagIssue.IssueId = issueId;
                        htagIssue.UserId = userId;
                        htagIssue.Action = "tag added (" + tag.Name + ")";
                        ctx2.Entry(htagIssue).State = EntityState.Added;
                        ctx2.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            ctx.Dispose();
            ctx2.Database.Connection.Close();
            ctx2.Dispose();
        }

        /// <summary>
        /// removes a list of tags from an issue
        /// </summary>
        /// <param name="tagList">list of tags</param>
        /// <param name="issueId">issue</param>
        public static void RemoveTagsFromIssue(List<Tag> tagList, int issueId, int userId)
        {
            TagIssue help;
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            foreach (Tag tag in tagList)
            {
                help = ctx.TagIssue.Find(tag.Id, issueId);
                ctx.TagIssue.Remove(help);
                ctx.Entry(help).State = EntityState.Deleted;
                ctx.SaveChanges();

                HTagIssue htagIssue = new HTagIssue();
                htagIssue.ChangeDate = DateTime.Now;
                htagIssue.TagId = tag.Id;
                htagIssue.IssueId = issueId;
                htagIssue.UserId = userId;
                htagIssue.Action = "tag deleted (" + tag.Name + ")";
                ctx.Entry(htagIssue).State = EntityState.Added;
                ctx.SaveChanges();
            }

            ctx.Dispose();
        }
    }
}
