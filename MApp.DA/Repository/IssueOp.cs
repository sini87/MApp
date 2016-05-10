using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    /// <summary>
    /// makes all operations to table Issue
    /// </summary>
    public class IssueOp
    {
        /// <summary>
        /// returns all isses which the user have access to
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<Issue> UserIssues(int userId)
        {
            User user = UserOp.GetUser(userId);
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            var query = from Issue in ctx.Issue.AsNoTracking()
                        where
                              (from AccessRight in ctx.AccessRight
                               where AccessRight.UserId == userId
                               select new
                               {
                                   AccessRight.IssueId  
                               }).Contains(new { IssueId = Issue.Id })
                        select Issue;
            List<Issue> list = new List<Issue>();
            foreach (Issue issue in query.AsNoTracking())
            {
                list.Add(issue);
            }

            ctx.Dispose();

            return list;
        }

        /// <summary>
        /// returns all available issues
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public static Issue GetIssueById(int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            Issue issue = ctx.Issue.AsNoTracking().Where(x => x.Id == issueId).FirstOrDefault();
            ctx.Dispose();

            return issue;
        }

        /// <summary>
        /// returns title of an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public static string IssueTitle(int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            string title = ctx.Issue.Where(x => x.Id == issueId).FirstOrDefault().Title;
            ctx.Dispose();
            return title;
        }

        /// <summary>
        /// updates an issue
        /// </summary>
        /// <param name="issue"></param>
        /// <param name="userId">user who is performing this operation</param>
        /// <returns></returns>
        public static int UpdateIssue(Issue issue, int userId)
        {
            Issue updateIssue;
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            updateIssue = ctx.Issue.Find(issue.Id);

            bool updated = false;
            if (issue.Title != updateIssue.Title)
            {
                updateIssue.Title = issue.Title;
                updated = true;
            }
            if (updateIssue.Description != issue.Description)
            {
                updateIssue.Description = issue.Description;
                updated = true;
            }
            if (updateIssue.AnonymousPosting != issue.AnonymousPosting)
            {
                updateIssue.AnonymousPosting = issue.AnonymousPosting;
                updated = true;
            }
            if (updateIssue.Status != issue.Status)
            {
                updateIssue.Status = issue.Status;
                updated = true;
            }
            if (updateIssue.Parent != issue.Parent)
            {
                updateIssue.Parent = issue.Parent;
                updated = true;
            }
            if (updateIssue.DependsOn != issue.DependsOn)
            {
                updateIssue.DependsOn = issue.DependsOn;
                updated = true;
            }
            if (updateIssue.Setting != issue.Setting)
            {
                updateIssue.Setting = issue.Setting;
                updated = true;
            }
            if (updated)
            {
                ctx.Entry(updateIssue).State = EntityState.Modified;
                ctx.SaveChanges();
            }

            ctx.Dispose();

            return updateIssue.Id;
        }

        /// <summary>
        /// inserts new issue
        /// </summary>
        /// <param name="issue"></param>
        /// <param name="userId">user who is performing operation</param>
        /// <returns></returns>
        public static int InsertIssue(Issue issue, int userId)
        {
            int issueId = -1;
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            try
            {
                Issue updateIssue = ctx.Issue.Create();
                updateIssue.Title = issue.Title;
                updateIssue.Description = issue.Description;
                updateIssue.AnonymousPosting = issue.AnonymousPosting;
                updateIssue.Status = issue.Status;
                updateIssue.Parent = issue.Parent;
                updateIssue.DependsOn = issue.DependsOn;
                updateIssue.Setting = issue.Setting;
                //updateIssue.TagIssue = null;
                ctx.Issue.Add(updateIssue);
                ctx.Entry(updateIssue).State = EntityState.Added;
                ctx.SaveChanges();
                issueId = updateIssue.Id;

                AccessRight ar = new AccessRight();
                ar.IssueId = issueId;
                ar.UserId = userId;
                ar.Right = "O";
                ar.SelfAssessmentValue = 10;
                ar.SelfAssesmentDescr = "";
                ar.MailNotification = false;
                ctx.AccessRight.Add(ar);
                ctx.Entry(ar).State = EntityState.Added;
                ctx.SaveChanges();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ctx.Dispose();

            return issueId;
        }

        /// <summary>
        /// returns root issues
        /// </summary>
        /// <returns></returns>
        public static List<Issue> RootIssues(int? issueId, ApplicationDBEntities ctx)
        {
            List<Issue> parentList = new List<Issue>();
            
            if (issueId == -1)
                return parentList;
            int? parent = ctx.Issue.Where(x => x.Id == issueId).FirstOrDefault().Parent;
            if (parent == null)
                return parentList;
            else
            {
                Issue parentIssue = ctx.Issue.AsNoTracking().Where(x => x.Id == issueId).FirstOrDefault();
                parentList.Add(parentIssue);
                List<Issue> recIssues = RootIssues(parentIssue.Parent, ctx);
                if (recIssues != null)
                {
                    parentList.AddRange(recIssues);
                }
            }

            return parentList;
        }

        /// <summary>
        /// deletes issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns>returns true if delete was successful</returns>
        public static bool DeleteIssue (int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            Issue issue = ctx.Issue.Find(issueId);
            ctx.Issue.Remove(issue);
            ctx.Entry(issue).State = EntityState.Deleted;
            try
            {
                ctx.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ctx.Dispose();

            return false;
        }

        /// <summary>
        /// puts issue to next stage
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId">user who is performing this action</param>
        public static void NextStage(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            Issue issue = ctx.Issue.Find(issueId);

            switch (issue.Status)
            {
                case "CREATING":
                    issue.Status = "BRAINSTORMING1";
                    break;
                case "BRAINSTORMING1":
                    issue.Status = "BRAINSTORMING2";
                    break;
                case "BRAINSTORMING2":
                    issue.Status = "EVALUATING";
                    break;
                case "EVALUATING":
                    issue.Status = "DECIDING";
                    break;
                case "DECIDING":
                    issue.Status = "FINISHED";
                    break;
            }
            ctx.Entry(issue).State = EntityState.Modified;
            ctx.SaveChanges();

            ctx.Dispose();
        }
    }
}
