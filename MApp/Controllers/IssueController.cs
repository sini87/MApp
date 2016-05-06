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
            vm.AllUsers = ic.GetAllUsers();

            if (issueId != -1)
            {
                vm.Issue = ic.GetIssue(issueId);
                vm.AccessRights = ic.GetAccessRightsOfIssue(issueId);
            }
            else
            {
                vm.Issue = new IssueModel();
                vm.Issue.Status = "CREATING";
                vm.Issue.Setting = "A";
                vm.Issue.AnonymousPosting = false;
                vm.AccessRights = new List<AccessRightModel>();
            }

            UserShortModel rmUser;
            foreach (AccessRightModel arm in vm.AccessRights)
            {
                rmUser = vm.AllUsers.Where(x => x.Id == arm.UserId).FirstOrDefault();
                vm.AllUsers.Remove(rmUser);
            }
            vm.AllUsers.Insert(0, new UserShortModel(0, "", ""));

            return View(vm);
        }

        [HttpPost]
        public ActionResult Creating([FromJson] CreatingVM creatingVM)
        {
            IssueCreating ic = new IssueCreating();
            int issueId = creatingVM.Issue.Id;
            int userId = GetUserIdFromClaim();
            creatingVM.Issue.Id = ic.SaveIssue(creatingVM.Issue, userId);
            ic.UpdateIsseuTags(creatingVM.Issue.Id, creatingVM.AddedTags, creatingVM.DeletedTags, userId);
            ic.UpdateAccessRights(creatingVM.AddedAR, creatingVM.DeletedAR, creatingVM.AccessRights, issueId, userId);
            return RedirectToAction("Creating", "Issue", new { issueId = creatingVM.Issue.Id });
        }

        [HttpPost]
        public ActionResult DeleteIssue(int issueId)
        {
            IssueCreating ic = new IssueCreating();
            ic.DeleteIssue(issueId);
            return RedirectToAction("Index","Issue");
        }

        [HttpPost]
        public ActionResult NextStage (int issueId)
        {
            IssueCreating ic = new IssueCreating();
            ic.NextStage(issueId,GetUserIdFromClaim());
            return RedirectToAction("BrCriteria","Issue",new { issueId = issueId });
        }

        [HttpGet]
        public ActionResult BrCriteria(int issueId)
        {
            IssueCreating ic = new IssueCreating();
            BrCriteriaVM viewModel = new BrCriteriaVM();
            viewModel.Issue = ic.GetIssue(issueId);
            IssueBrCriteria ibc = new IssueBrCriteria();
            viewModel.IssueCriteria = ibc.GetIssueCriteria(issueId, GetUserIdFromClaim());
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult BrCriteria ([FromJson]BrCriteriaVM brCriteriaVM)
        {
            IssueBrCriteria ibc = new IssueBrCriteria();
            ibc.UpdateCriteria(brCriteriaVM.IssueCriteria, brCriteriaVM.DeletedCriteria, GetUserIdFromClaim());
            brCriteriaVM.IssueCriteria = ibc.GetIssueCriteria(brCriteriaVM.Issue.Id, GetUserIdFromClaim());
            return View(brCriteriaVM);
        }

        [HttpGet]
        public ActionResult BrAlternatives(int issueId)
        {
            BrAlternativesVM vm = new BrAlternativesVM();
            IssueCreating ic = new IssueCreating();
            vm.Issue = ic.GetIssue(issueId);
            return View(vm);
        }

        public ActionResult CriteriaRating(int issueId)
        {
            CriteriaRatingVM vm = new CriteriaRatingVM();
            IssueCreating ic = new IssueCreating();
            vm.Issue = ic.GetIssue(issueId);
            return View(vm);
        }
    }
}