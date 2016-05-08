using MApp.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class RatingModel : ListModel<Rating,RatingModel>, IListModel<Rating,RatingModel>
    {
        public int CriterionId { get; set; }
        public int UserId { get; set; }
        public int AlternativeId { get; set; }
        public double Value { get; set; }

        public Rating ToEntity(RatingModel model)
        {
            Rating entity = new Rating();
            entity.AlternativeId = model.AlternativeId;
            entity.CriterionId = model.CriterionId;
            entity.UserId = model.UserId;
            entity.Value = model.Value;
            return entity;
        }

        public RatingModel ToModel(Rating entity)
        {
            RatingModel model = new RatingModel();
            model.CriterionId = entity.CriterionId;
            model.UserId = entity.UserId;
            model.AlternativeId = entity.AlternativeId;
            model.Value = entity.Value;
            return model;
        }
    }
}
