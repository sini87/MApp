using MApp.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class CriterionWeightModel : ListModel<CriterionWeight,CriterionWeightModel>, IListModel<CriterionWeight,CriterionWeightModel>
    {
        public int UserId { get; set; }
        public int CriterionId { get; set; }
        public double Weight { get; set; }
        public string Name { get; set; }

        public CriterionWeight ToEntity(CriterionWeightModel model)
        {
            CriterionWeight entity = new CriterionWeight();
            entity.CriterionId = model.CriterionId;
            entity.UserId = model.UserId;
            entity.Weight = model.Weight;
            return entity;
        }

        public CriterionWeightModel ToModel(CriterionWeight entity)
        {
            CriterionWeightModel model = new CriterionWeightModel();
            model.UserId = entity.UserId;
            model.CriterionId = entity.CriterionId;
            model.Weight = entity.Weight;
            return model;
        }
    }
}
