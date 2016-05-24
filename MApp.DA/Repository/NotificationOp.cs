using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class NotificationOp
    {
        public static List<Notification> GetGroupthinkNotifications(int issueId, int userId)
        {
            List<Notification> list;
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            list = ctx.Notification.AsNoTracking().Where(x => x.IssueId == issueId && x.Type == "Groupthink").ToList();

            ctx.Dispose();
            return list;
        }

        public static int AddNotification(Notification notification)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            Notification newNot = ctx.Notification.Create();
            int id;
            newNot.IssueId = notification.IssueId;
            newNot.UserId = notification.UserId;
            newNot.Type = notification.Type;
            newNot.Text = notification.Text;
            newNot.Read = false;
            newNot.AddedDate = System.DateTime.Now;
            ctx.Entry(newNot).State = EntityState.Added;
            ctx.SaveChanges();
            id = newNot.Id;
            ctx.Dispose();
            return id;
        }

        /// <summary>
        /// marks an notification as read
        /// </summary>
        /// <param name="notificationId"></param>
        public static void MarkNotificationAsRead(int notificationId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            Notification not = ctx.Notification.Find(notificationId);
            not.Read = true;
            ctx.Entry(not).State = EntityState.Modified;
            ctx.SaveChanges();
            ctx.Dispose();
        }

        /// <summary>
        /// returns a list of key-value pairs
        /// the key is the name of the property
        /// the value is a list of users who share the property/skill
        /// the threshold is currently 50%
        /// so if 50% of the users of an issue share the same property 
        /// this method will return these properties with the users
        /// only 'Owners' and 'Contributors' are considered, not owners
        /// minimum 2 users must have accessright to the issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, List<string>>> GetGroupshiftProperties(int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<KeyValuePair<string, List<string>>> list = new List<KeyValuePair<string, List<string>>>();

            if (ctx.AccessRight.Where(x => x.IssueId == issueId && x.Right != "V").Count() < 2)
            {
                return list;
            }

            string query = "select CAST(count(*) AS FLOAT) / CAST((Select count(*) FROM AccessRight Where IssueId = " + issueId + " AND [Right] NOT LIKE 'V') AS FLOAT) "
                + "AS 'Percentage', p.Name, p.Id "
                + "From Property p, userProperty up, AccessRight Ar "
                + "where Ar.IssueId = " + issueId + " AND "
                + "ar.UserId = up.UserId AND "
                + "up.PropertyId = p.Id AND "
                + "[Right] NOT LIKE 'V' "
                + "GROUP BY p.Name, p.Id "
                + "ORDER BY 'Percentage' DESC";

            ApplicationDBEntities ctx2 = new ApplicationDBEntities();
            DbCommand cmd = ctx2.Database.Connection.CreateCommand();
            ctx2.Database.Connection.Open();
            string propertyName;
            double val;
            int propertyId;
            KeyValuePair<string, List<string>> kvp;
            cmd.CommandText = query;
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    val = reader.GetDouble(0);
                    if (val >= 0.5)
                    {
                        propertyName = reader.GetString(1);
                        propertyId = reader.GetInt32(2);

                        List<User> userList;
                        List<string> userNameList = new List<string>();
                        string queryUser = "SELECT * from appSchema.[User] u WHERE u.Id in " +
                            "(Select up.UserId from " +
                            "UserProperty up, AccessRight ar " +
                            "Where up.PropertyId = {0} AND " +
                            "ar.UserId = up.UserId AND " +
                            "ar.IssueId = {1})";
                        userList = ctx.Database.SqlQuery<User>(queryUser, propertyId, issueId).ToList();
                        foreach (User u in userList)
                        {
                            userNameList.Add(u.FirstName + " " + u.LastName);
                        }
                        kvp = new KeyValuePair<string, List<string>>(propertyName, userNameList);
                        list.Add(kvp);
                    }
                    else
                    {
                        reader.Close();
                        cmd.Dispose();
                        break;
                    }
                }

            }
            ctx.Dispose();
            ctx2.Dispose();
            return list;
        }

        /// <summary>
        /// returns user with most changes made to an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns>key value pair, key is user name and value the count of changes</returns>
        public static KeyValuePair<string,int> UserWithMostChanges(int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            KeyValuePair<string, int> kvp = new KeyValuePair<string, int>();

            var changes = ctx.Changes_View.AsNoTracking().Where(x => x.IssueId == 10).GroupBy(info => info.UserId)
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
        public static List<KeyValuePair<int,int>> GetAllChangeCountsByUser(int issueId)
        {
            List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            var best = ctx.Changes_View.AsNoTracking().Where(x => x.IssueId == 10).GroupBy(info => info.UserId)
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
    }
}
