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
            Tag help;
            TagIssue ti;
            foreach (Tag tag in tagList)
            {
                if (tag.Id == -1)
                {
                    help = Ctx.Tag.Create();
                    help.Name = tag.Name;
                    Ctx.Entry(help).State = EntityState.Added;
                    Ctx.SaveChanges();
                    tag.Id = help.Id;
                }
                ti = Ctx.TagIssue.Create();
                ti.TagId = tag.Id;
                ti.IssueId = issueId;
                Ctx.Entry(ti).State = EntityState.Added;
                Ctx.SaveChanges();
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
