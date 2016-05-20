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
            return list.OrderBy(x => x.CriterionId).ToList();
        }

        public void SaveCriterionWeights(List<CriterionWeightModel> criteriaWeights, int issueId, int userId)
        {
            CriterionWeightModel cwm = new CriterionWeightModel();
            List<CriterionWeight> entityList = cwm.ToEntityList(criteriaWeights);
            CriterionWeightOp.SaveCriterionWeights(entityList, issueId, userId);
        }

        public List<CriterionWeightModel>[] GetIssueWeights(int issueId, int userId)
        {
            CriterionWeightModel cwm = new CriterionWeightModel();
            List<CriterionWeightModel> allWeightsList = cwm.ToModelList(CriterionWeightOp.GetIssueWeights(issueId, userId), cwm);
            allWeightsList = allWeightsList.Where(x => x.UserId != userId).ToList() ;
            IssueCreating ic = new IssueCreating();
            List<UserShortModel> allUsers = ic.GetAllUsers();
            List<int> distinctUsers = allWeightsList.GroupBy(x => x.UserId).Select(grp => grp.First()).Select(x => x.UserId).ToList();
            List<CriterionWeightModel>[] arrayList = new List<CriterionWeightModel>[distinctUsers.Count];

            int cnt = 0;
            foreach(int uId in distinctUsers)
            {
                arrayList[cnt] = new List<CriterionWeightModel>();
                string name = allUsers.Where(x => x.Id == uId).FirstOrDefault().Name;
                foreach(CriterionWeightModel model in allWeightsList.Where(x => x.UserId == uId).OrderBy(x => x.CriterionId))
                {
                    model.Name = name;
                    arrayList[cnt].Add(model);
                }
                cnt++;
            }

            return arrayList;
        }
    }
}
