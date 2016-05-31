using MApp.DA;
using MApp.DA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class PairwiseComparisonCriterionModel : ListModel<PairwiseComparisonCC, PairwiseComparisonCriterionModel>, IListModel<PairwiseComparisonCC, PairwiseComparisonCriterionModel>
    {
        public int LeftCritId { get; set; }
        public string LeftCritName { get; set; }
        public int RightCritId { get; set; }
        public string RightCritName { get; set; }
        public int UserId { get; set; }
        public string Value { get; set; }

        public PairwiseComparisonCC ToEntity(PairwiseComparisonCriterionModel model)
        {
            Dictionary<double, string> values = PairwiseComparisonOp.Values;
            PairwiseComparisonCC pcc;
            pcc = new PairwiseComparisonCC();
            pcc.CriterionLeft = model.LeftCritId;
            pcc.CriterionRight = model.RightCritId;
            pcc.UserId = model.UserId;
            pcc.Value = values.Where(x => x.Value == model.Value).FirstOrDefault().Key;
            return pcc;
        }

        public PairwiseComparisonCriterionModel ToModel(PairwiseComparisonCC entity)
        {
            PairwiseComparisonCriterionModel pccm = new PairwiseComparisonCriterionModel();
            Dictionary<double, string> values = PairwiseComparisonOp.Values;
            pccm.LeftCritId = entity.CriterionLeft;
            pccm.RightCritId = entity.CriterionRight;
            pccm.Value = values.Where(x => x.Key == entity.Value).FirstOrDefault().Value;
            return pccm;
        }
    }
}
