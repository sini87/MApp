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
        public static List<Issue> UserIssues(int userId)
        {
            User user = UserOp.GetUser(userId);

            string q = "WITH myrec AS " +
                "SELECT Id, Parent, Title, 1 AS 'Level' " +
                "FROM [appSchema].Issue AS a " +
                "WHERE a.Parent IS NULL " +
                "UNION ALL " +
                "Select b.Id, b.Parent, b.Title, a.Level + 1 " +
                "FROM myrec as a " +
                "INNER JOIN [appSchema].Issue AS b ON b.Parent = a.id " +
                "WHERE b.Parent IS NOT NULL) " +
                "SELECT DISTINCT Id, Parent, Title, Level From myrec";

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
            //Ctx.Entry(updateIssue).State = EntityState.Modified;
            Ctx.SaveChanges();
            return updateIssue.Id;
        }

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
                updateIssue.TagIssue = null;
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
        /// returns Ids of users who have access to an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public static Dictionary<int,string> GetAccessRightsForIssue(int issueId)
        {
            Dictionary<int,string> list= new Dictionary<int,string>();
            var query = from AccessRight in Ctx.AccessRight
                        where
                          AccessRight.IssueId == issueId
                        select new
                        {
                            AccessRight.UserId,
                            AccessRight.Right
                        };
            foreach (var ent in query)
            {
                list.Add(ent.UserId, ent.Right);
            }

            return list;
        }
    }
}