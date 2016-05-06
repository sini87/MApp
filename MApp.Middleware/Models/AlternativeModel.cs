using MApp.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class AlternativeModel : ListModel<Alternative, AlternativeModel>, IListModel<Alternative, AlternativeModel>
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public double? Rating { get; set; }
        public Alternative ToEntity(AlternativeModel model)
        {
            Alternative alt = new Alternative();
            alt.Id = model.Id;
            alt.IssueId = model.IssueId;
            alt.Name = model.Name;
            alt.Description = model.Description;
            alt.Reason = model.Reason;
            alt.Rating = model.Rating;
            return alt;
        }

        public AlternativeModel ToModel(Alternative entity)
        {
            AlternativeModel model = new AlternativeModel();
            model.Id = entity.Id;
            model.Name = entity.Name;
            model.IssueId = entity.IssueId;
            model.Description = entity.Description;
            model.Reason = entity.Reason;
            model.Rating = entity.Rating;
            return model;
        }
    }
}
