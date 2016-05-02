using System;
using System.Collections.Generic;
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
            foreach (TagIssue ti in Ctx.Issue.Find(issueId).TagIssue)
            {
                list.Add(ti.Tag);
            }
            return list;
        }
    }
}
