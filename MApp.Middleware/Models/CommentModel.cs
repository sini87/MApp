using MApp.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class CommentModel : ListModel<Comment, CommentModel>, IListModel<Comment,CommentModel>
    {
        public long DateTime { get; set; }
        public int IssueId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public bool Anonymous { get; set; }
        public string Name { get; set; }

        public Comment ToEntity(CommentModel model)
        {
            Comment entity = new Comment();
            entity.Anonymous = model.Anonymous;
            entity.IssueId = model.IssueId;
            entity.Text = model.Text;
            entity.Type = model.Type;
            entity.UserId = model.UserId;
            return entity;
        }

        public CommentModel ToModel(Comment entity)
        {
            CommentModel model = new CommentModel();
            model.UserId = entity.UserId;
            model.Type = entity.Type;
            model.Text = entity.Text;
            model.IssueId = entity.IssueId;
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0);
            var unixDateTime = (entity.DateTime.Ticks - epoch.Ticks) / TimeSpan.TicksPerMillisecond - Convert.ToInt32(TimeZone.CurrentTimeZone.GetUtcOffset(entity.DateTime).TotalMilliseconds);
            model.DateTime = unixDateTime;
            return model;
        }
    }
}
