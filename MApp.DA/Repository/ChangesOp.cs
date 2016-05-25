﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class ChangesOp
    {
        /// <summary>
        /// returns user with most changes made to an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns>key value pair, key is user name and value the count of changes</returns>
        public static KeyValuePair<string, int> UserWithMostChanges(int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            KeyValuePair<string, int> kvp = new KeyValuePair<string, int>();

            var changes = ctx.Changes_View.AsNoTracking().Where(x => x.IssueId == issueId).GroupBy(info => info.UserId)
                .Select(group => new
                {
                    UserId = group.Key,
                    Count = group.Count()
                })
                    .OrderByDescending(x => x.Count);
            if (changes.Count() > 0)
            {
                User u = new User();
                var x = changes.FirstOrDefault();
                u = ctx.User.Find(x.UserId);
                kvp = new KeyValuePair<string, int>(u.FirstName + " " + u.LastName, x.Count);
            }

            ctx.Dispose();
            return kvp;
        }

        /// <summary>
        /// returns users with the count of changes they made
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns>list of key value pairs, key is the userId and value the count of changes</returns>
        public static List<KeyValuePair<int, int>> GetAllChangeCountsByUser(int issueId)
        {
            List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            var best = ctx.Changes_View.AsNoTracking().Where(x => x.IssueId == issueId).GroupBy(info => info.UserId)
                .Select(group => new
                {
                    UserId = group.Key,
                    Count = group.Count()
                })
                    .OrderBy(x => x.Count);

            KeyValuePair<int, int> kvp;
            foreach (var vp in best)
            {
                kvp = new KeyValuePair<int, int>(vp.UserId, vp.Count);
                list.Add(kvp);
            }

            ctx.Dispose();
            return list;
        }

        /// <summary>
        /// returns the count of user changes for an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int GetUserChangesCount(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            int cnt = 0;
            var best = ctx.Changes_View.AsNoTracking().Where(x => x.IssueId == issueId && x.UserId == userId).GroupBy(info => info.UserId)
                .Select(group => new
                {
                    UserId = group.Key,
                    Count = group.Count()
                })
                    .OrderBy(x => x.Count);

            foreach (var vp in best)
            {
                cnt = vp.Count;
            }

            ctx.Dispose();
            return cnt;
        }

        public static List<Changes_View> GetUserChanges(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            List<Changes_View> list = ctx.Changes_View.AsNoTracking().Where(x => x.IssueId == issueId && x.UserId == userId).OrderByDescending(x => x.ChangeDate).ToList();

            return list;
        }
    }
}
