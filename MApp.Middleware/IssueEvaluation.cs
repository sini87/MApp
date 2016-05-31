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
    public class IssueEvaluation
    {
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

        public List<RatingModel> GetAllIssueRatings (int issueId, int userId)
        {
            List<RatingModel> userRatings;
            RatingModel rm = new RatingModel();
            userRatings = rm.ToModelList(RatingOp.GetAllIssueRatings(issueId, userId), rm);
            return userRatings;
        }

        public List<AlternativeModel> GetIssueAlternatives(int issueId, int userId)
        {
            AlternativeModel am = new AlternativeModel();
            List<AlternativeModel> list = am.ToModelList(AlternativeOp.GetIssueAlternatives(issueId, userId),am);
            return list;
        }

        public List<CriterionModel> GetIssueCrtieria(int issueId, int userId)
        {
            CriterionModel cm = new CriterionModel();
            List<CriterionModel> list = cm.ToModelList(CriterionOp.GetIssueCriterions(issueId, userId), cm);
            return list;
        }

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

        public List<PairwiseComparisonRatingModel> GetPairwiseAlternativeRatings(int issueId, int userId)
        {
            PairwiseComparisonRatingModel pcacm = new PairwiseComparisonRatingModel();
            List<PairwiseComparisonRatingModel> modelList = pcacm.ToModelList(PairwiseComparisonOp.GetAlternativeComparison(issueId, userId), pcacm);
            List<Criterion> cList = CriterionOp.GetIssueCriterions(issueId, userId);

            //if user already compared alternatives then return him that
            if (modelList != null && modelList.Count > 0)
            {
                foreach(PairwiseComparisonRatingModel model in modelList)
                {
                    model.CriterionName = cList.Find(x => x.Id == model.CriterionId).Name;
                }
                return modelList;
            }else // else construct the comparisons
            {
                List<Alternative> aList = AlternativeOp.GetIssueAlternatives(issueId, userId);
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
                            pcacm.Value = "Equally important";
                            pcacm.UserId = userId;
                            modelList.Add(pcacm);
                        }
                    }
                }

                return modelList;
            }
        }

        public string SaveAHPAlternativeEvaluation(List<PairwiseComparisonRatingModel> list, int issueId, int userId)
        {
            PairwiseComparisonRatingModel pcrm = new PairwiseComparisonRatingModel();
            List<PairwiseComparisonAC> entityList = pcrm.ToEntityList(list);
            return PairwiseComparisonOp.SaveAlternativeComparison(issueId, userId, entityList);
        }
    }
}
