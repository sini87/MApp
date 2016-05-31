using MApp.DA;
using MApp.DA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class PairwiseComparisonRatingModel : ListModel<PairwiseComparisonAC, PairwiseComparisonRatingModel>, IListModel<PairwiseComparisonAC,PairwiseComparisonRatingModel>
    {
        public int CriterionId { get; set; }
        public string CriterionName { get; set; }
        public int LeftAltId { get; set; }
        public string LeftAltName { get; set; }
        public int RightAltId { get; set; }
        public string RightAltName { get; set; }
        public int UserId { get; set; }
        public string Value { get; set; }

        public PairwiseComparisonAC ToEntity(PairwiseComparisonRatingModel model)
        {
            Dictionary<double, string> values = PairwiseComparisonOp.Values;
            PairwiseComparisonAC pcac = new PairwiseComparisonAC();
            pcac.AlternativeLeft = model.LeftAltId;
            pcac.AlternativeRight = model.RightAltId;
            pcac.UserId = model.UserId;
            pcac.Value = values.Where(x => x.Value == model.Value).FirstOrDefault().Key;
            pcac.CriterionId = model.CriterionId;
            return pcac;
        }  

        public PairwiseComparisonRatingModel ToModel(PairwiseComparisonAC entity)
        {
            Dictionary<double, string> values = PairwiseComparisonOp.Values;
            PairwiseComparisonRatingModel model = new PairwiseComparisonRatingModel();
            model.LeftAltId = entity.AlternativeLeft;
            model.RightAltId = entity.AlternativeRight;
            model.UserId = entity.UserId;
            model.Value = values.Where(x => x.Key == entity.Value).FirstOrDefault().Value;
            model.CriterionId = entity.CriterionId;
            return model;
        }
    }
}
