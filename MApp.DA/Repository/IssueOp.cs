using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public static int UpdateIssue(Issue issue, int userId)
        {
            Issue updateIssue;
            updateIssue = GetIssueById(issue.Id);
            if (issue.Title != updateIssue.Title)
            {
                updateIssue.Title = issue.Title;
            }
            if (updateIssue.Description != issue.Description)
            {
                updateIssue.Description = issue.Description;
            }
            if (updateIssue.AnonymousPosting != issue.AnonymousPosting)
            {
                updateIssue.AnonymousPosting = issue.AnonymousPosting;
            }
            if (updateIssue.Status != issue.Status)
            {
                updateIssue.Status = issue.Status;
            }
            if (updateIssue.Parent != issue.Parent)
            {
                updateIssue.Parent = issue.Parent;
            }
            if (updateIssue.DependsOn != issue.DependsOn)
            {
                updateIssue.DependsOn = issue.DependsOn;
            }
            if (updateIssue.Setting != issue.Setting)
            {
                updateIssue.Setting = issue.Setting;
            }
            Ctx.Entry(updateIssue).State = EntityState.Modified;
            Ctx.SaveChanges();
            return updateIssue.Id;
        }

        public static int InsertIssue(Issue issue, int userId)
        {
            Issue updateIssue = Ctx.Issue.Create();
            updateIssue.Title = issue.Title;
            updateIssue.Description = issue.Description;
            updateIssue.AnonymousPosting = issue.AnonymousPosting;
            updateIssue.Status = issue.Status;
            updateIssue.Parent = issue.Parent;
            updateIssue.DependsOn = issue.DependsOn;
            updateIssue.Setting = issue.Setting;
            Ctx.Issue.Add(updateIssue);
            Ctx.Entry(updateIssue).State = EntityState.Added;
            Ctx.SaveChanges();

            using (var dbContextTransaction = Ctx.Database.BeginTransaction())
            {
                Ctx.Database.ExecuteSqlCommand("INSERT INTO [appSchema].[AccessRight] VALUES({0},{1},'O',NULL,NULL,NULL,0,NULL)",userId,updateIssue.Id);
                dbContextTransaction.Commit();
            }
            return updateIssue.Id;
        }
    }
}
