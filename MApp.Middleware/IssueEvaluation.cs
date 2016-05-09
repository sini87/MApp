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
            IssueBrAlternative iba = new IssueBrAlternative();
            return iba.GetIssueAlternatives(issueId, userId);
        }

        public List<CriterionModel> GetIssueCrtieria(int issueId, int userId)
        {
            IssueBrCriteria ibc = new IssueBrCriteria();
            return ibc.GetIssueCriteria(issueId, userId);
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
    }
}
