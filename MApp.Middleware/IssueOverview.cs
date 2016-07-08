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
    /// middleware class for issues overview
    /// </summary>
    public class IssueOverview
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns>list of all issues which the user can access</returns>
        public List<IssueModel> GetUserIssues(int userId)
        {
            IssueModel im = new IssueModel();
            List<IssueModel> allIssues = im.ToModelList(IssueOp.UserIssues(userId), im);
            List<IssueModel> rootIssues = new List<IssueModel>();

            foreach (IssueModel model in allIssues.Where(m => m.Parent == null))
            {
                TagModel tm = new TagModel();
                model.Tags = tm.ToModelList(TagOp.GetIssueTags(model.Id), tm);
                model.Children = ChildIssues(allIssues, model.Id);
                rootIssues.Add(model);
            }

            List<IssueModel> hlist = new List<IssueModel>();
            foreach (IssueModel m in rootIssues)
            {
                Traverse(m, m.Children, ref hlist);
            }

            return hlist;
        }

        /// <summary>
        /// returns list of issues which the user has access to
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns>list of user issue model</returns>
        public List<UserIssueModel> GetUIM (int userId)
        {
            List<IssueModel> list = GetUserIssues(userId);
            List<UserIssueModel> uimList = new List<UserIssueModel>();
            UserIssueModel uim;
            List<KeyValuePair<string, int>> unreadInfos;
            int unreadCnt;

            foreach(IssueModel im in list)
            {
                uim = GetUserIssueModelFromIssueModel(im, userId);
                uimList.Add(uim);
            }

            return uimList;
        }

        /// <summary>
        /// converts inssue model to user issue model
        /// </summary>
        /// <param name="im">issue model</param>
        /// <param name="userId">user who is performing operation</param>
        /// <returns>user issue model</returns>
        private UserIssueModel GetUserIssueModelFromIssueModel(IssueModel im, int userId)
        {
            UserIssueModel uim = new UserIssueModel();
            int unreadCnt;
            List<KeyValuePair<string, int>> unreadInfos;

            uim.Issue = im;
            uim.SelfAssessmentActionRequired = AccessRightOp.SelfAssessmentActionRequired(im.Id, userId);
            uim.CriteriaActionRatingRequired = CriterionOp.CriteriaWeightingActionRequired(im.Id, userId);
            uim.EvaluationActionRequired = RatingOp.GetRatingActionRequired(im.Id, userId);

            uim.UnreadCoreItems = new List<string>();
            unreadInfos = InformationReadOp.GetUnreadInfos(im.Id, userId);
            unreadCnt = 0;
            foreach (KeyValuePair<string, int> kvp in unreadInfos)
            {
                if (kvp.Key.StartsWith("Alternative I") || kvp.Key.StartsWith("Issue I") || kvp.Key.StartsWith("Criteria I"))
                {
                    unreadCnt = unreadCnt + kvp.Value;
                    if (kvp.Value > 0)
                    {
                        if (kvp.Key.StartsWith("Alternative Information"))
                        {
                            uim.UnreadCoreItems.Add(kvp.Value + " new Alternatives");
                        }
                        if (kvp.Key.StartsWith("Issue Information"))
                        {
                            uim.UnreadCoreItems.Add(kvp.Value + " new Issue Attributes");
                        }
                        if (kvp.Key.StartsWith("Criteria Information"))
                        {
                            uim.UnreadCoreItems.Add(kvp.Value + " new Criteria");
                        }
                    }
                }
            }
            uim.UnreadCoreItemsCount = unreadCnt;
            TagModel tm = new TagModel();
            if (uim.Issue.Tags == null || uim.Issue.Tags.Count == 0)
            {
                uim.Issue.Tags = tm.ToModelList(TagOp.GetIssueTags(uim.Issue.Id), tm);
            }

            ReviewModel rm = new ReviewModel();
            IssueCreating ic = new IssueCreating();
            List<UserShortModel> userList = ic.GetAllUsers();
            UserShortModel usm;

            if (uim.Issue.Status == "FINISHED" || uim.Issue.Status == "CLOSED")
            {
                uim.Rating = ReviewOp.GetReviewRating(uim.Issue.Id);
                uim.Reviews = rm.ToModelList(ReviewOp.GetIssueReviews(uim.Issue.Id), rm);
                foreach (ReviewModel reviewModel in uim.Reviews)
                {
                    usm = userList.Find(x => x.Id == reviewModel.UserId);
                    reviewModel.UserName = usm.FirstName + " " + usm.LastName;
                }
            }
            else
            {
                uim.Rating = 0.0;
            }

            return uim;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user who is performing operation</param>
        /// <returns>user issue model by issue id</returns>
        public UserIssueModel GetUserIssueModel(int issueId, int userId)
        {
            IssueModel im = new IssueModel();
            im = im.ToModel(IssueOp.GetIssueById(issueId));
            return GetUserIssueModelFromIssueModel(im, userId);
        }

        private void Traverse(IssueModel root, List<IssueModel> node, ref List<IssueModel> list)
        {
            list.Add(root);
            if (node != null)
            {
                foreach (IssueModel m in node)
                {
                    Traverse(m, m.Children, ref list);
                }
            }
        }

        private List<IssueModel> ChildIssues(List<IssueModel> list, int issueId)
        {
            List<IssueModel> children = new List<IssueModel>();
            foreach (IssueModel model in list.Where(m => m.Parent == issueId))
            {
                model.Children = ChildIssues(list, model.Id);
                TagModel tm = new TagModel();
                model.Tags = tm.ToModelList(TagOp.GetIssueTags(model.Id), tm); 
                children.Add(model);
            }
            if (children.Count == 0)
            {
                return null;
            }
            else
            {
                return children;
            }

        }

        /// <summary>
        /// saves an issue review
        /// </summary>
        /// <param name="reviewModel">issue review</param>
        public void SaveIssueReview(ReviewModel reviewModel)
        {
            ReviewOp.SaveIssueReview(reviewModel.ToEntity(reviewModel));
        }
    }
}
