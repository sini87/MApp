using MApp.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class NotificationModel : ListModel<Notification, NotificationModel>, IListModel<Notification, NotificationModel>
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public bool Read { get; set; }
        public long AddedDate { get; set; }

        public Notification ToEntity(NotificationModel model)
        {
            Notification entity = new Notification();
            entity.Id = model.Id;
            entity.IssueId = model.IssueId;
            entity.UserId = model.UserId;
            entity.Type = model.Type;
            entity.Text = model.Text;
            entity.Read = model.Read;
            return entity;
        }

        public NotificationModel ToModel(Notification entity)
        {
            NotificationModel model = new NotificationModel();
            model.Id = entity.Id;
            model.IssueId = entity.IssueId;
            model.UserId = entity.UserId;
            model.Type = entity.Type;
            model.Text = entity.Text;
            if (entity.Read == null || entity.Read == false)
            {
                model.Read = false;
            }else
            {
                model.Read = true;
            }
            if (entity.AddedDate.HasValue)
            {
                DateTime dt = entity.AddedDate.Value;
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0);
                var unixDateTime = (dt.Ticks - epoch.Ticks) / TimeSpan.TicksPerMillisecond - Convert.ToInt32(TimeZone.CurrentTimeZone.GetUtcOffset(entity.AddedDate.Value).TotalMilliseconds);
                model.AddedDate = unixDateTime;
            }
            return model;
        }
    }
}
