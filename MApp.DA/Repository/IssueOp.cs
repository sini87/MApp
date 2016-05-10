﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class IssueOp : Operations
    {
        /// <summary>
        /// returns all isses which the user have access to
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// returns all available issues
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public static Issue GetIssueById(int issueId)
        {
            return Ctx.Issue.Find(issueId);
        }

        /// <summary>
        /// returns title of an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public static string IssueTitle(int issueId)
        {
            return Ctx.Issue.Find(issueId).Title;
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
            updateIssue = GetIssueById(issue.Id);
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
                Ctx.Entry(updateIssue).State = EntityState.Modified;
                Ctx.SaveChanges();
            }
            
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
            try
            {
                Issue updateIssue = Ctx.Issue.Create();
                updateIssue.Title = issue.Title;
                updateIssue.Description = issue.Description;
                updateIssue.AnonymousPosting = issue.AnonymousPosting;
                updateIssue.Status = issue.Status;
                updateIssue.Parent = issue.Parent;
                updateIssue.DependsOn = issue.DependsOn;
                updateIssue.Setting = issue.Setting;
                //updateIssue.TagIssue = null;
                Ctx.Issue.Add(updateIssue);
                Ctx.Entry(updateIssue).State = EntityState.Added;
                Ctx.SaveChanges();
                issueId = updateIssue.Id;

                AccessRight ar = new AccessRight();
                ar.IssueId = issueId;
                ar.UserId = userId;
                ar.Right = "O";
                ar.SelfAssessmentValue = 10;
                ar.SelfAssesmentDescr = "";
                ar.MailNotification = false;
                Ctx.AccessRight.Add(ar);
                Ctx.Entry(ar).State = EntityState.Added;
                Ctx.SaveChanges();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return issueId;
        }

        /// <summary>
        /// returns root issues
        /// </summary>
        /// <returns></returns>
        public static List<Issue> RootIssues(int issueId)
        {
            List<Issue> parentList = new List<Issue>();
            if (issueId == -1)
                return parentList;
            int? parent = Ctx.Issue.Find(issueId).Parent;
            if (parent == null)
                return null;
            else
            {
                Issue parentIssue = Ctx.Issue.Find(parent);
                parentList.Add(parentIssue);
                List<Issue> recIssues = RootIssues(parentIssue.Id);
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
            Issue issue = Ctx.Issue.Find(issueId);
            Ctx.Issue.Remove(issue);
            Ctx.Entry(issue).State = EntityState.Deleted;
            try
            {
                Ctx.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// puts issue to next stage
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId">user who is performing this action</param>
        public static void NextStage(int issueId, int userId)
        {
            Issue issue = Ctx.Issue.Find(issueId);

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
            Ctx.Entry(issue).State = EntityState.Modified;
            Ctx.SaveChanges();
        }
    }
}
