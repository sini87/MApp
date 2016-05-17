using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public static void AddNotification(Notification notification)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            Notification newNot = ctx.Notification.Create();

            newNot.IssueId = notification.IssueId;
            newNot.UserId = notification.UserId;
            newNot.Type = notification.Type;
            newNot.Text = notification.Text;
            newNot.Read = false;
            newNot.AddedDate = System.DateTime.Now;
            ctx.Entry(newNot).State = EntityState.Added;
            ctx.SaveChanges();
            ctx.Dispose();
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
    }
}
