using MApp.DA.Repository;
using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware
{
    public class IssueOverview
    {
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

            return uim;
        }

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
    }
}
