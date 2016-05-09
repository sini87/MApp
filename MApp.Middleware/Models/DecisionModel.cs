using MApp.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class DecisionModel : ListModel<Decision,DecisionModel>, IListModel<Decision,DecisionModel>
    {
        public int IssueId { get; set; }
        public int AlternativeId { get; set; }
        public string Explanation { get; set; }
        public DateTime ChangeDate { get; set; }

        public Decision ToEntity(DecisionModel model)
        {
            Decision entity = new Decision();
            entity.IssueId = model.IssueId;
            entity.AlternativeId = model.AlternativeId;
            entity.Explanation = model.Explanation;
            return entity;
        }

        public DecisionModel ToModel(Decision entity)
        {
            DecisionModel model = new DecisionModel();
            model.IssueId = entity.IssueId;
            model.AlternativeId = Convert.ToInt32(entity.AlternativeId);
            model.Explanation = entity.Explanation;
            return model;
        }
    }
}
