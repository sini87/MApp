using MApp.DA.Repository;
using MApp.Web.Models;
using MApp.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MApp.Web.Controllers
{
    public class IssueController : Controller
    {
        // GET: Issue
        public ActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.SerialNumber).Value);
            IssueModel im = new IssueModel();
            List<IssueModel> allIssues = im.ToModelList(IssueOp.UserIssues(userId),im);
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

            return View(hlist);
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

        private List<IssueModel> RootOrder (List<IssueModel>list, int ? issueId)
        {
            List<IssueModel> retL = new List<IssueModel>();
            retL.Add(list.Where(i => i.Id == issueId).FirstOrDefault());

            if (issueId == null)
            {
                foreach (IssueModel im in list)
                {
                    foreach (IssueModel m in list.Where(i => i.Parent == im.Id).ToList())
                    {
                        retL.AddRange(RootOrder(list, m.Id));
                    }

                }
            }else
            {
                foreach (IssueModel m in list.Where(i => i.Parent == issueId).ToList())
                {
                    retL.AddRange(RootOrder(list, m.Id));
                }
            }
            
                
            return retL;
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
            }else
            {
                return children;
            }

        }
    }
}