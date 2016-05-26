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
            List<Issue> l = ctx.Database.SqlQuery<Issue>("SELECT * FROM Issue WHERE Id IN (SELECT IssueId FROM AccessRight Where UserId =" + userId + ")").ToList();
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

            return l;
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
            List<string> updatedFields = new List<string>();

            bool updated = false;
            if (issue.Title != updateIssue.Title)
            {
                updateIssue.Title = issue.Title;
                updated = true;
                updatedFields.Add("title");
            }
            if (updateIssue.Description != issue.Description)
            {
                updateIssue.Description = issue.Description;
                updated = true;
                updatedFields.Add("description");
            }
            if (updateIssue.AnonymousPosting != issue.AnonymousPosting)
            {
                updateIssue.AnonymousPosting = issue.AnonymousPosting;
                updated = true;
                updatedFields.Add("anonymous posting");
            }
            if (updateIssue.Status != issue.Status)
            {
                updateIssue.Status = issue.Status;
                updated = true;
                updatedFields.Add("status");
            }
            if (updateIssue.Parent != issue.Parent)
            {
                updateIssue.Parent = issue.Parent;
                updated = true;
                updatedFields.Add("parent issue");
            }
            if (updateIssue.DependsOn != issue.DependsOn)
            {
                updateIssue.DependsOn = issue.DependsOn;
                updated = true;
                updatedFields.Add("depends on issue");
            }
            if (updateIssue.Setting != issue.Setting)
            {
                updateIssue.Setting = issue.Setting;
                updated = true;
                updatedFields.Add("setting");
            }
            if (updated)
            {
                ctx.Entry(updateIssue).State = EntityState.Modified;
                ctx.SaveChanges();
                HIssue hissue = new HIssue();
                hissue.ChangeDate = DateTime.Now;
                hissue.IssueId = issue.Id;
                hissue.UserId = userId;
                hissue.Action = "Issue updated (";
                bool first = true;
                foreach (string str in updatedFields)
                {
                    if (first)
                    {
                        hissue.Action = hissue.Action + str;
                        first = false;
                    }
                    else
                    {
                        hissue.Action = hissue.Action + " ," + str;
                    }
                }
                hissue.Action = hissue.Action + ")";
                hissue.Status = issue.Status;
                hissue.Title = issue.Title;
                hissue.Description = issue.Description;
                hissue.Setting = issue.Setting;
                hissue.Status = issue.Status;
                hissue.AnonymousPosting = issue.AnonymousPosting;
                hissue.Parent = issue.Parent;
                hissue.DependsOn = issue.DependsOn;
                hissue.GroupThink = issue.GroupThink;
                hissue.ReviewRating = issue.ReviewRating;

                //problem with action 
                hissue.Action = "issue updated";

                ctx.HIssue.Add(hissue);
                ctx.Entry(hissue).State = EntityState.Added;
                try
                {
                    ctx.SaveChanges();
                }catch(DbEntityValidationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                


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
        public static int InsertIssue(Issue issue, int userId, double selfAssessmentValue, string selfAssessmentDescription)
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
                ar.SelfAssessmentValue = selfAssessmentValue;
                ar.SelfAssesmentDescr = selfAssessmentDescription;
                ar.MailNotification = false;
                ctx.AccessRight.Add(ar);
                ctx.Entry(ar).State = EntityState.Added;

                HAccessRight har = new HAccessRight();
                har.ChangeDate = DateTime.Now;
                har.IssueId = ar.IssueId;
                har.UserId = userId;
                har.Action = "selfassessment added";
                har.SelfAssesmentDescr = selfAssessmentDescription;
                har.SelfAssessmentValue = selfAssessmentValue;
                ctx.HAccessRight.Add(har);
                ctx.Entry(har).State = EntityState.Added;

                HIssue hissue = new HIssue();
                hissue.ChangeDate = DateTime.Now;
                hissue.IssueId = issueId;
                hissue.UserId = userId;
                hissue.Action = "issue created";
                hissue.Status = issue.Status;
                hissue.Title = issue.Title;
                hissue.Description = issue.Description;
                hissue.Setting = issue.Setting;
                hissue.Status = issue.Status;
                hissue.AnonymousPosting = issue.AnonymousPosting;
                hissue.Parent = issue.Parent;
                hissue.DependsOn = issue.DependsOn;
                hissue.GroupThink = issue.GroupThink;
                hissue.ReviewRating = issue.ReviewRating;
                ctx.HIssue.Add(hissue);
                ctx.Entry(hissue).State = EntityState.Added;

                ctx.SaveChanges();

                //mark issue as read
                ApplicationDBEntities ctx2 = new ApplicationDBEntities();
                DbCommand cmd = ctx2.Database.Connection.CreateCommand();
                ctx2.Database.Connection.Open();
                cmd.CommandText = "UPDATE appSchema.InformationRead SET [Read] = 1 WHERE UserId = " + userId + " AND TName LIKE 'Alternative' AND FK LIKE '" + issueId + "'";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();
                ctx2.Database.Connection.Close();
                ctx2.Dispose();
            }
            catch (DbEntityValidationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
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
        public static bool DeleteIssue(int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            Issue issue = ctx.Issue.Find(issueId);
            ctx.Issue.Remove(issue);
            ctx.Entry(issue).State = EntityState.Deleted;
            try
            {
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
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

            HIssue hissue = new HIssue();
            hissue.ChangeDate = DateTime.Now;
            hissue.IssueId = issueId;
            hissue.UserId = userId;
            hissue.Action = "issue moved to next stage";
            hissue.Status = issue.Status;
            hissue.Title = issue.Title;
            hissue.Description = issue.Description;
            hissue.Setting = issue.Setting;
            hissue.Status = issue.Status;
            hissue.AnonymousPosting = issue.AnonymousPosting;
            hissue.Parent = issue.Parent;
            hissue.DependsOn = issue.DependsOn;
            hissue.GroupThink = issue.GroupThink;
            hissue.ReviewRating = issue.ReviewRating;
            ctx.HIssue.Add(hissue);
            ctx.Entry(hissue).State = EntityState.Added;
            ctx.SaveChanges();

            ctx.Dispose();
        }
    }
}
