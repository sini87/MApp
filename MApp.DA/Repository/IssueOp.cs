using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class IssueOp : Operations
    {
        public static List<Issue> UserIssues(int userId)
        {
            User user = UserOp.GetUser(userId);

            //var query = from issue in Ctx.Issue
            //            join ar in Ctx.AccessRight on issue.Id equals ar.IssueId
            //            join u in Ctx.User on ar.UserId equals userId
            //            select issue;


            var query = from Issue in Ctx.Issue
                        where
                              (from AccessRight in Ctx.AccessRight
                               where AccessRight.UserId == userId
                               select new
                               {
                                   AccessRight.IssueId
                               }).Contains(new { IssueId = Issue.Id })
                        select Issue;
            List<Issue> list = new List<Issue>();
            foreach (Issue issue in query)
            {
                list.Add(issue);
            }
            return list;
        }

        public static Issue GetIssueById(int issueId)
        {
            return Ctx.Issue.Find(issueId);
        }

        public static string IssueTitle(int issueId)
        {
            return Ctx.Issue.Find(issueId).Title;
        }
    }
}
