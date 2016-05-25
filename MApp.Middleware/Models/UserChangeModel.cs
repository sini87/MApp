using MApp.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class UserChangeModel : ListModel<Changes_View, UserChangeModel>, IListModel<Changes_View, UserChangeModel>
    {
        public long ChangeDate { get; set; }
        public int IssueId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }

        public Changes_View ToEntity(UserChangeModel model)
        {
            return new Changes_View();
        }

        public UserChangeModel ToModel(Changes_View entity)
        {
            UserChangeModel model = new UserChangeModel();
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0);
            var unixDateTime = (entity.ChangeDate.Ticks - epoch.Ticks) / TimeSpan.TicksPerMillisecond - Convert.ToInt32(TimeZone.CurrentTimeZone.GetUtcOffset(entity.ChangeDate).TotalMilliseconds);
            model.ChangeDate = unixDateTime;
            model.IssueId = entity.IssueId;
            model.UserId = entity.UserId;
            model.Action = entity.Action;
            return model;
        }
    }
}
