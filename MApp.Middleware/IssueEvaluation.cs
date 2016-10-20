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
    /// middleware class for alternative evaluation
    /// </summary>
    public class IssueEvaluation
    {
        /// <summary>
        /// returns user ratings
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns>array of lists of userRatings, one array stands for one alternative</returns>
        public List<RatingModel>[] GetIssueUserRatings(int issueId, int userId)
        {
            List<RatingModel>[] retArr;
            IssueBrCriteria ibc = new IssueBrCriteria();
            int critCnt = ibc.GetIssueCriteria(issueId, userId).Count;
            retArr = new List<RatingModel>[critCnt];
            IssueBrAlternative iba = new IssueBrAlternative();
            int altcnt =  iba.GetIssueAlternatives(issueId, userId).Count;
            List<RatingModel> userRatings;
            RatingModel rm = new RatingModel();
            userRatings = rm.ToModelList(RatingOp.GetUserRatings(issueId, userId), rm);

            int rowCnt = 0;
            int colCnt = 0;
            retArr[0] = new List<RatingModel>();
            foreach (RatingModel rat in userRatings)
            {
                if (colCnt == altcnt)
                {
                    colCnt = 0;
                    rowCnt++;
                    retArr[rowCnt] = new List<RatingModel>();
                }

                retArr[rowCnt].Add(rat);

                colCnt++;
            }

            return retArr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user who is performing operation</param>
        /// <returns>alternative ratings/evaluations of all other users</returns>
        public List<RatingModel> GetAllIssueRatings (int issueId, int userId)
        {
            List<RatingModel> userRatings;
            RatingModel rm = new RatingModel();
            userRatings = rm.ToModelList(RatingOp.GetAllIssueRatings(issueId, userId), rm);
            return userRatings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user who is performing this operation</param>
        /// <returns>list of alternatives</returns>
        public List<AlternativeModel> GetIssueAlternatives(int issueId, int userId)
        {
            AlternativeModel am = new AlternativeModel();
            List<AlternativeModel> list = am.ToModelList(AlternativeOp.GetIssueAlternatives(issueId, userId),am);
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user who is performing operation</param>
        /// <returns>list of criteria</returns>
        public List<CriterionModel> GetIssueCrtieria(int issueId, int userId)
        {
            CriterionModel cm = new CriterionModel();
            List<CriterionModel> list = cm.ToModelList(CriterionOp.GetIssueCriterions(issueId, userId), cm);
            return list;
        }

        /// <summary>
        /// saves ratings
        /// </summary>
        /// <param name="ratings">array of lists where an list represents an alternative rating</param>
        public void SaveUserRatings(List<RatingModel>[] ratings)
        {
            List<RatingModel> ratList = new List<RatingModel>();
            
            for (int i = 0; i < ratings.Length; i++)
            {
                foreach (RatingModel rat in ratings[i])
                {
                    ratList.Add(rat);
                }
            }

            RatingModel rm = new RatingModel();
            List<Rating> entityList = rm.ToEntityList(ratList);
            RatingOp.SaveUserRatings(entityList);
        }

        /// <summary>
        /// returns list of users who have already rated
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user id who is performing operation</param>
        /// <returns>list of users</returns>
        public List<UserShortModel> GetRatedUsersForIssue(int issueId, int userId)
        {
            List<int> userIds = RatingOp.GetAlreadyRatedUsers(issueId, userId);
            IssueCreating ic = new IssueCreating();
            List<UserShortModel> allUsers = ic.GetAllUsers();
            List<UserShortModel> ratedUsers = new List<UserShortModel>();
            foreach(int id in userIds)
            {
                ratedUsers.Add(allUsers.Where(x => x.Id == id).FirstOrDefault());
            }
            return ratedUsers;
        }

        /// <summary>
        /// returns list of pairwise comparisons
        /// if user has not made any comparisons yet then this method will construct empty comparisons
        /// else return is the comparisons made
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user who is perfomring operation</param>
        /// <returns>list of pairwise comparisons</returns>
        public List<PairwiseComparisonRatingModel> GetPairwiseAlternativeRatings(int issueId, int userId)
        {
            PairwiseComparisonRatingModel pcacm = new PairwiseComparisonRatingModel();
            List<PairwiseComparisonRatingModel> modelList = pcacm.ToModelList(PairwiseComparisonOp.GetAlternativeComparison(issueId, userId), pcacm);
            List<Criterion> cList = CriterionOp.GetIssueCriterions(issueId, userId);
            List<Alternative> aList = AlternativeOp.GetIssueAlternatives(issueId, userId);

            //if user already compared alternatives then return him that
            if (modelList != null && modelList.Count > 0)
            {
                foreach(PairwiseComparisonRatingModel model in modelList)
                {
                    model.CriterionName = cList.Find(x => x.Id == model.CriterionId).Name;
                    model.LeftAltName = aList.Find(x => x.Id == model.LeftAltId).Name;
                    model.RightAltName = aList.Find(x => x.Id == model.RightAltId).Name;
                    model.Value = model.Value.Replace("important", "preferred");
                }
                return modelList;
            }else // else construct the comparisons
            {
                modelList = new List<PairwiseComparisonRatingModel>();
                foreach(Criterion crit in cList)
                {
                    for(int i = 0; i < aList.Count; i++)
                    {
                        for(int j = i + 1; j < aList.Count; j++)
                        {
                            pcacm = new PairwiseComparisonRatingModel();
                            pcacm.CriterionId = crit.Id;
                            pcacm.CriterionName = crit.Name;
                            pcacm.LeftAltId = aList[i].Id;
                            pcacm.LeftAltName = aList[i].Name;
                            pcacm.RightAltId = aList[j].Id;
                            pcacm.RightAltName = aList[j].Name;
                            pcacm.Value = "Equally preferred";
                            pcacm.UserId = userId;
                            modelList.Add(pcacm);
                        }
                    }
                }

                return modelList;
            }
        }

        /// <summary>
        /// saves ratings for setting AHP
        /// </summary>
        /// <param name="list">list of user pairweise ratings</param>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user who is performing operation</param>
        /// <returns>returns success if save was corrent, else detaild consistency error message for user</returns>
        public string SaveAHPAlternativeEvaluation(List<PairwiseComparisonRatingModel> list, int issueId, int userId)
        {
            PairwiseComparisonRatingModel pcrm = new PairwiseComparisonRatingModel();
            List<PairwiseComparisonAC> entityList = pcrm.ToEntityList(list);
            return PairwiseComparisonOp.SaveAlternativeComparison(issueId, userId, entityList);
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
                array[i] = val.Value.Replace("important","preferred");
                i++;
            }
            return array;
        }
    }
}
