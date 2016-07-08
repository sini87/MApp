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
    /// <summary>
    /// middleware class for criteria weighting
    /// </summary>
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

        /// <summary>
        /// saves criterion weight for an user
        /// </summary>
        /// <param name="criteriaWeights">list of criterion weights</param>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user id</param>
        public void SaveCriterionWeights(List<CriterionWeightModel> criteriaWeights, int issueId, int userId)
        {
            CriterionWeightModel cwm = new CriterionWeightModel();
            List<CriterionWeight> entityList = cwm.ToEntityList(criteriaWeights);
            CriterionWeightOp.SaveCriterionWeights(entityList, issueId, userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user id who is performing operation</param>
        /// <returns>array of lists containging user criteria weights</returns>
        public List<CriterionWeightModel>[] GetIssueWeights(int issueId, int userId)
        {
            CriterionWeightModel cwm = new CriterionWeightModel();
            List<CriterionWeightModel> allWeightsList = cwm.ToModelList(CriterionWeightOp.GetIssueWeights(issueId, userId), cwm);
            //allWeightsList = allWeightsList.Where(x => x.UserId != userId).ToList() ;
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>string array of slider values for pairwise comparison</returns>
        public string[] GetSliderValues()
        {
            Dictionary<double, string> values = PairwiseComparisonOp.Values;
            string[] array = new string[values.Count];
            int i = 0;
            foreach (var val in values)
            {
                array[i] = val.Value;
                i++;
            }
            return array;
        }

        /// <summary>
        /// if user has not compared this method calculates the needed comparisons and returns it
        /// else it will return a list of already compared criterias
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns>list of criteria pairwise comparison</returns>
        public List<PairwiseComparisonCriterionModel> GetPCCriteria (int issueId, int userId)
        {
            List<Criterion> cList = CriterionOp.GetIssueCriterions(issueId, userId);
            PairwiseComparisonCriterionModel pccm = new PairwiseComparisonCriterionModel();
            List<PairwiseComparisonCriterionModel> list = pccm.ToModelList(PairwiseComparisonOp.GetWeightComparison(issueId, userId), pccm);
            
            //if user has compared criteria
            //construct empty weights
            if (list.Count() == 0)
            {
                list = new List<PairwiseComparisonCriterionModel>();
                for (int i = 0; i < cList.Count; i++)
                {
                    for (int j = i + 1; j < cList.Count; j++)
                    {
                        pccm = new PairwiseComparisonCriterionModel();
                        pccm.LeftCritId = cList[i].Id;
                        pccm.LeftCritName = cList[i].Name;
                        pccm.RightCritId = cList[j].Id;
                        pccm.RightCritName = cList[j].Name;
                        pccm.Value = "Equally important";
                        list.Add(pccm);
                    }
                }
            }else
            {
                foreach(var model in list)
                {
                    model.LeftCritName = cList.Find(x => x.Id == model.LeftCritId).Name;
                    model.RightCritName = cList.Find(x => x.Id == model.RightCritId).Name;
                }
            }
            
            return list;
        }

        /// <summary>
        /// returns true if comparison was saved an consistency check OK
        /// </summary>
        /// <param name="pccmList"></param>
        /// <param name="userId"></param>
        /// <param name="userName">name of user</param>
        /// <returns></returns>
        public List<CriterionWeightModel> SavePCCriteria(List<PairwiseComparisonCriterionModel> pccmList, int userId, string userName)
        {
            PairwiseComparisonCC pcc;
            List<PairwiseComparisonCC> pccList = new List<PairwiseComparisonCC>();
            Dictionary<double, string> values = PairwiseComparisonOp.Values;
            foreach (PairwiseComparisonCriterionModel pccm in pccmList)
            {
                pcc = new PairwiseComparisonCC();
                pcc.CriterionLeft = pccm.LeftCritId;
                pcc.CriterionRight = pccm.RightCritId;
                pcc.UserId = userId;
                pcc.Value = values.Where(x => x.Value == pccm.Value).FirstOrDefault().Key;
                pccList.Add(pcc);
            }

            CriterionWeightModel cwm = new CriterionWeightModel();
            List<CriterionWeightModel> cwList = cwm.ToModelList(PairwiseComparisonOp.SaveWeightComparison(pccList), cwm);
            foreach(var model in cwList)
            {
                model.Name = userName;
            }

            return cwList;
        }
    }
}
