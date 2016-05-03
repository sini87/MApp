using MApp.Middleware;
using MApp.Middleware.Models;
using MApp.Web.CustomLibraries;
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
        private int GetUserIdFromClaim()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.SerialNumber).Value);
            return userId;
        }

        // GET: Issue
        public ActionResult Index()
        {
            int userId = GetUserIdFromClaim();          
            IssueOverview iO = new IssueOverview();

            return View(iO.GetUserIssues(userId));
        }


        public ActionResult Creating (int issueId)
        {
            CreatingVM vm = new CreatingVM();
            IssueCreating ic = new IssueCreating();
            int userId = GetUserIdFromClaim();
            vm.AllTags = ic.GetAllTags();
            vm.Issues = new List<IssueShort>();
            vm.Issues.Add(new IssueShort(-1, "none"));
            vm.Issues.AddRange(ic.GetUserIssuesShort(userId));

            if (issueId != -1)
            {
                vm.Issue = ic.GetIssue(issueId);
            }else
            {
                vm.Issue = new IssueModel();
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult Creating([FromJson] CreatingVM creatingVM)
        {
            IssueCreating ic = new IssueCreating();
            creatingVM.Issue.Id = ic.SaveIssue(creatingVM.Issue, GetUserIdFromClaim());
            ic.UpdateIsseuTags(creatingVM.Issue.Id, creatingVM.AddedTags, creatingVM.DeletedTags,GetUserIdFromClaim());
            creatingVM.Tags = ic.GetIssueTags(creatingVM.Issue.Id);

            //TagModel tmp;
            //foreach(TagModel t in creatingVM.DeletedTags)
            //{
            //    if (creatingVM.Issue.Tags.Where(x => x.Id == t.Id && x.Name == t.Name).Count() > 0)
            //    {
            //        tmp = creatingVM.Issue.Tags.Where(x => x.Id == t.Id).FirstOrDefault();
            //        creatingVM.Issue.Tags.Remove(tmp);
            //    }
            //}
            //foreach (TagModel t in creatingVM.AddedTags)
            //{
            //    if (creatingVM.Issue.Tags.Where(x => x.Id == t.Id).Count() == 0)
            //    {
            //        creatingVM.Issue.Tags.Add(t);
            //    }
            //}


            return View(creatingVM);
        }
    }
}