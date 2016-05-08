using MApp.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    /// <summary>
    /// Criterion model representing criterion entity
    /// </summary>
    public class CriterionModel : ListModel<Criterion,CriterionModel>, IListModel<Criterion,CriterionModel>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IssueId { get; set; }
        public double? Weight { get; set; }
        public double? WeightPC { get; set; }

        public Criterion ToEntity(CriterionModel model)
        {
            Criterion entity = new Criterion();
            entity.Id = model.Id;
            entity.Description = model.Description;
            entity.Issue = model.IssueId;
            entity.Name = model.Name;
            return entity;
        }

        public CriterionModel ToModel(Criterion entity)
        {
            CriterionModel model = new CriterionModel();
            model.Id = entity.Id;
            model.Description = entity.Description;
            model.IssueId = entity.Issue;
            model.Name = entity.Name;
            model.Weight = entity.Weight;
            model.WeightPC = entity.WeightPC;
            return model;
        }
    }
}
