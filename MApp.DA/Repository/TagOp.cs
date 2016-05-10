using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public static List<Tag> GetIssueTags (int issueId)
        {
            List <Tag> list = new List<Tag>();
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            foreach (TagIssue ti in ctx.TagIssue.AsNoTracking().Where(ti => ti.IssueId == issueId).ToList())
            {
                list.Add(ti.Tag);
            }

            ctx.Dispose();

            return list;
        }

        public static void AddTagsToIssue(List<Tag> tagList, int issueId)
        {
            string sql;
            ApplicationDBEntities ctx = new ApplicationDBEntities();

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
                    sql = "INSERT INTO appSchema.[TagIssue] VALUES (" + tag.Id + "," + issueId +")";
                    ctx.Database.ExecuteSqlCommand(sql, tag.Id,issueId  );
                    try
                    {
                        dbContextTransaction.Commit();
                    }catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            ctx.Dispose();
        }

        /// <summary>
        /// removes a list of tags from an issue
        /// </summary>
        /// <param name="tagList"></param>
        /// <param name="issueId"></param>
        public static void RemoveTagsFromIssue(List<Tag> tagList, int issueId)
        {
            TagIssue help;
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            foreach (Tag tag in tagList)
            {
                help = ctx.TagIssue.Find(tag.Id, issueId);
                ctx.TagIssue.Remove(help);
                ctx.Entry(help).State = EntityState.Deleted;
                ctx.SaveChanges();
            }

            ctx.Dispose();
        }
    }
}
