using MApp.DA;
using MApp.DA.Repository;
using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware
{
    public class IssueCriterionWeight
    {
        /// <summary>
        /// return user weights of specified issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CriterionWeightModel> GetUserWeights(int issueId, int userId)
        {
            List<CriterionWeight> entityList;
            List<CriterionWeightModel> list;
            CriterionWeightModel cwModel = new CriterionWeightModel();
            entityList = CriterionWeightOp.GetIssueWeightsOfUser(issueId, userId);

            if (entityList.Count > 0)
            {
                list = cwModel.ToModelList(entityList, cwModel);
            }else
            {
                list = cwModel.ToModelList(CriterionWeightOp.GetEmptyWeights(issueId, userId), cwModel);
            }

            List<Criterion> cList = CriterionOp.GetIssueCriterions(issueId, userId);
            foreach (CriterionWeightModel cwm in list)
            {
                cwm.Name = cList.Where(x => x.Id == cwm.CriterionId).FirstOrDefault().Name;
            }
            return list;
        }
    }
}
