using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class AccessRightOp : Operations
    {
        /// <summary>
        /// returns Ids of users who have access to an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetAccessRightsForIssue(int issueId)
        {
            Dictionary<int, string> list = new Dictionary<int, string>();
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

        public static void UpdateRights(List<AccessRight> addedList, List<AccessRight> deletedList, List<AccessRight> editedList, int issueId, int userId)
        {
            List<AccessRight> intAddList = addedList.Intersect(Ctx.AccessRight).ToList();
            deletedList = deletedList.Distinct().ToList();
            addedList = addedList.Except(intAddList).ToList();

            foreach (AccessRight ar in addedList)
            {
                ar.IssueId = issueId;
                ar.MailNotification = false;
                ar.NotificationLevel = "";
                ar.SelfAssesmentDescr = "";
                Ctx.AccessRight.Add(ar);
                Ctx.Entry(ar).State = EntityState.Added;
                try
                {
                    Ctx.SaveChanges();
                }catch(DbEntityValidationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                GrantAccess(ar.UserId, issueId);
                
            }

            foreach (AccessRight ar in deletedList)
            {
                if (Ctx.AccessRight.Where(x => x.UserId == ar.UserId && x.IssueId == issueId).Count() > 0)
                {
                    using (var dbContextTransaction = Ctx.Database.BeginTransaction())
                    {
                        Ctx.Database.ExecuteSqlCommand("delete from [appSchema].[AccessRight] WHERE UserId = {0} AND IssueId ={1}", ar.UserId, issueId);
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
                tmp = Ctx.AccessRight.Where(x => x.UserId == ar.UserId && x.IssueId == ar.IssueId).FirstOrDefault();
                if (tmp.Right != ar.Right)
                {
                    tmp.Right = ar.Right;
                    Ctx.Entry(tmp).State = EntityState.Modified;
                    Ctx.SaveChanges();
                }
            }
        }

        /// <summary>
        /// grants view access to all parent issues
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="issueId"></param>
        private static void GrantAccess(int userId, int issueId)
        {
            List<Issue> parentList = IssueOp.RootIssues(issueId);

            foreach (Issue i in parentList)
            {
                if (i.AccessRight.Where(x => x.UserId == userId).Count() == 0)
                {
                    AccessRight ar = new AccessRight();
                    ar.UserId = userId;
                    ar.IssueId = i.Id;
                    ar.Right = "O";
                    ar.MailNotification = false;
                    ar.NotificationLevel = "";
                    ar.SelfAssesmentDescr = "";
                    Ctx.AccessRight.Add(ar);
                    Ctx.Entry(ar).State = EntityState.Added;
                    try
                    {
                        Ctx.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public static string AccessRightOfUserForIssue(int userId, int issueId)
        {
            return Ctx.AccessRight.Where(x => x.IssueId == issueId && x.UserId == userId).FirstOrDefault().Right;
        }
    }
}
