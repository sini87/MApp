﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    /// <summary>
    /// handles all db operations to Entity AccessRight
    /// </summary>
    public class AccessRightOp
    {
        /// <summary>
        /// returns Ids of users who have access to an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public static List<AccessRight> GetAccessRightsForIssue(int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<AccessRight> list;
            var query = from AccessRight in ctx.AccessRight
                        where
                          AccessRight.IssueId == issueId
                        select AccessRight;

            list = query.AsNoTracking().ToList();

            ctx.Dispose();

            return list;
        }

        /// <summary>
        /// updates the accessrights for an issue
        /// </summary>
        /// <param name="addedList">new access rights added to issue</param>
        /// <param name="deletedList">accessrights which should be deleted</param>
        /// <param name="editedList">list of edited access rights</param>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        public static void UpdateRights(List<AccessRight> addedList, List<AccessRight> deletedList, List<AccessRight> editedList, int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            List<AccessRight> intAddList = addedList.Intersect(ctx.AccessRight).ToList();
            deletedList = deletedList.Distinct().ToList();
            addedList = addedList.Except(intAddList).ToList();

            foreach (AccessRight ar in addedList)
            {
                ar.IssueId = issueId;
                ar.MailNotification = false;
                ar.NotificationLevel = "";
                ar.SelfAssesmentDescr = "";
                ar.SelfAssessmentValue = 0;
                ctx.AccessRight.Add(ar);
                ctx.Entry(ar).State = EntityState.Added;
                try
                {
                    ctx.SaveChanges();
                }catch(DbEntityValidationException ex)
                {
                    Console.WriteLine(ex.Message);
                    ctx.AccessRight.Remove(ar);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ctx.AccessRight.Remove(ar);
                }
                GrantAccess(ar.UserId, issueId, ctx);
                
            }

            foreach (AccessRight ar in deletedList)
            {
                if (ctx.AccessRight.Where(x => x.UserId == ar.UserId && x.IssueId == issueId).Count() > 0)
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        ctx.Database.ExecuteSqlCommand("delete from [appSchema].[AccessRight] WHERE UserId = {0} AND IssueId ={1}", ar.UserId, issueId);
                        dbContextTransaction.Commit();
                    }
                }
            }
            

            if (editedList == null)
            {
                return;
            }
            AccessRight tmp;
            foreach (AccessRight ar in editedList)
            {
                if (ar.IssueId != 0)
                {
                    tmp = ctx.AccessRight.AsNoTracking().Where(x => x.UserId == ar.UserId && x.IssueId == issueId).FirstOrDefault();
                    if (tmp != null && tmp.Right != ar.Right)
                    {
                        tmp.Right = ar.Right;
                        ctx.Entry(tmp).State = EntityState.Modified;
                        ctx.SaveChanges();
                    }
                }
            }

            ctx.Dispose();
        }

        /// <summary>
        /// grants view access to all parent issues
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="issueId"></param>
        private static void GrantAccess(int userId, int issueId, ApplicationDBEntities ctx)
        {
            List<Issue> parentList = IssueOp.RootIssues(issueId, ctx);

            foreach (Issue i in parentList)
            {
                if (i.AccessRight.Where(x => x.UserId == userId).Count() == 0)
                {
                    AccessRight ar = new AccessRight();
                    ar.UserId = userId;
                    ar.IssueId = i.Id;
                    ar.Right = "V";
                    ar.MailNotification = false;
                    ar.NotificationLevel = "";
                    ar.SelfAssesmentDescr = "";
                    ar.SelfAssessmentValue = 0;
                    ctx.AccessRight.Add(ar);
                    ctx.Entry(ar).State = EntityState.Added;
                    try
                    {
                        ctx.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// gets the accessright for user of an issue
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public static AccessRight AccessRightOfUserForIssue(int userId, int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            AccessRight right = ctx.AccessRight.AsNoTracking().Where(x => x.IssueId == issueId && x.UserId == userId).FirstOrDefault();
            ctx.Dispose();
            return right;
        }

        /// <summary>
        /// updates slefassesment of an User
        /// </summary>
        /// <param name="value"></param>
        /// <param name="description"></param>
        public static void UpdateSelfAssesment(double value, string description, int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            AccessRight right = ctx.AccessRight.AsNoTracking().Where(x => x.IssueId == issueId && x.UserId == userId).FirstOrDefault();
            bool update = false;

            if (right.SelfAssessmentValue != value)
            {
                right.SelfAssessmentValue = value;
                update = true;
            }
            if (right.SelfAssesmentDescr != description)
            {
                right.SelfAssesmentDescr = description;
                update = true;
            }
            if (update)
            {
                ctx.Entry(right).State = EntityState.Modified;
                ctx.SaveChanges();
            }

            ctx.Dispose();
        }
    }
}
