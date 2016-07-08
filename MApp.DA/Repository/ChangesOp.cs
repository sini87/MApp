using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    /// <summary>
    /// class where all operation to changes view are made
    /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user id</param>
        /// <returns>list of changes made from user for issue</returns>
        public static List<Changes_View> GetUserChanges(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<Changes_View> list = ctx.Changes_View.AsNoTracking().Where(x => x.IssueId == issueId && x.UserId == userId).OrderByDescending(x => x.ChangeDate).ToList();
            ctx.Dispose();
            return list;
        }

        /// <summary>
        /// returns last 100 changes made
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public static List<Changes_View> GetLast100Changes(int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<Changes_View> list = ctx.Changes_View.AsNoTracking().Where(x => x.IssueId == issueId).OrderByDescending(x => x.ChangeDate).ToList();
            if (list.Count > 100)
            {
                list =  list.Take(100).ToList();
            }
            ctx.Dispose();
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <returns>last change made to issue</returns>
        public static Changes_View LastChange(int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<Changes_View> list = ctx.Changes_View.AsNoTracking().Where(x => x.IssueId == issueId).OrderByDescending(x => x.ChangeDate).ToList();
            Changes_View cv = new Changes_View();
            if (list.Count > 0)
            {
                cv = list.First();
            }
            return cv;
        }

        /// <summary>
        /// returns a list of users with changes made
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns>List of tuples where first item is user name and second item the number of changes</returns>
        public static List<KeyValuePair<string,int>>GetGroupActivity(int issueId)
        {
            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            string query = "Select cw.UserId, Count(*) AS 'Changes' from " +
                "Changes_View cw, [User] u WHERE " +
                "cw.IssueId = " + issueId + " AND " +
                "u.Id = cw.UserId " +
                "GROUP BY cw.UserId " +
                "UNION " +
                "Select ar.UserId, 0 AS 'Changes' FROM " +
                "AccessRight ar " +
                "WHERE ar.IssueId = " + issueId + " AND ar.[Right] != 'V' AND " +
                "ar.UserId NOT IN (Select DISTINCT(UserId) FROM Changes_View Where IssueId = " + issueId + ") " +
                "order By Changes Asc";

            User u;
            List<User> userList = ctx.User.ToList();
            DbCommand cmd = ctx.Database.Connection.CreateCommand();
            ctx.Database.Connection.Open();
            cmd.CommandText = query;
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    u = userList.Find(x => x.Id == reader.GetInt32(0));
                    list.Add(new KeyValuePair<string, int>(u.FirstName + ' ' + u.LastName, reader.GetInt32(1)));
                }
                reader.Close();
            }

            ctx.Dispose();
            return list;
        }
    }
}
