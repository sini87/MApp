using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class TagOp : Operations
    {
        /// <summary>
        /// returns all available Tags
        /// </summary>
        /// <returns></returns>
        public static List<Tag> GetAllTags()
        {
            return Ctx.Tag.ToList();
        }

        /// <summary>
        /// returns all tags of Issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public static List<Tag> GetIssueTags (int issueId)
        {
            List <Tag> list = new List<Tag>();
            
            foreach (TagIssue ti in Ctx.TagIssue.Where(ti => ti.IssueId == issueId).ToList())
            {
                list.Add(ti.Tag);
            }
            return list;
        }

        public static void AddTagsToIssue(List<Tag> tagList, int issueId)
        {
            string sql;
            foreach (Tag tag in tagList)
            {
                using (var dbContextTransaction = Ctx.Database.BeginTransaction())
                {
                    if (tag.Id == -1)
                    {
                        sql = "INSERT INTO appSchema.[Tag] (Name) OUTPUT INSERTED.Id VALUES ({0})";
                        var res = Ctx.Database.SqlQuery<int>(sql, tag.Name);
                        dbContextTransaction.Commit();
                        tag.Id = res.FirstOrDefault();
                    }
                    sql = "INSERT INTO appSchema.[TagIssue] VALUES (" + tag.Id + "," + issueId +")";
                    Ctx.Database.ExecuteSqlCommand(sql, tag.Id,issueId  );
                    try
                    {
                        dbContextTransaction.Commit();
                    }catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    
                }
            }
        }

        public static void RemoveTagsFromIssue(List<Tag> tagList, int issueId)
        {
            TagIssue help;
            foreach (Tag tag in tagList)
            {
                help = Ctx.TagIssue.Find(tag.Id, issueId);
                Ctx.TagIssue.Remove(help);
                Ctx.Entry(help).State = EntityState.Deleted;
                Ctx.SaveChanges();
            }
        }
    }
}
