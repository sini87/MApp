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
