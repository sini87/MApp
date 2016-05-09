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
            brCriteriaVM.DeletedCriteria = new List<int>();
            return View(brCriteriaVM);
        }

        [HttpGet]
        public ActionResult BrAlternatives(int issueId)
        {
            BrAlternativesVM vm = new BrAlternativesVM();
            IssueCreating ic = new IssueCreating();
            int userId = GetUserIdFromClaim();
            vm.Issue = ic.GetIssue(issueId);
            IssueBrAlternative iba = new IssueBrAlternative();
            vm.Alternatives = iba.GetIssueAlternatives(issueId, userId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult BrAlternatives([FromJson] BrAlternativesVM brAlternativesVM)
        {
            IssueBrAlternative iba = new IssueBrAlternative();
            int userId = GetUserIdFromClaim();
            iba.UpdateAlternatives(brAlternativesVM.Alternatives, brAlternativesVM.DeletedAlternatives, userId);
            brAlternativesVM.Alternatives = iba.GetIssueAlternatives(brAlternativesVM.Issue.Id, userId);
            brAlternativesVM.DeletedAlternatives = new List<int>();
            return View(brAlternativesVM);
        }

        public ActionResult CriteriaRating(int issueId)
        {
            CriteriaWeightsVM vm = new CriteriaWeightsVM();
            IssueCreating ic = new IssueCreating();
            IssueCriterionWeight icw = new IssueCriterionWeight();
            int userId = GetUserIdFromClaim();
            vm.Issue = ic.GetIssue(issueId);
            vm.UserWeights = icw.GetUserWeights(issueId, userId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult CriteriaRating([FromJson] CriteriaWeightsVM criteriaWeightsVM)
        {
            IssueCriterionWeight icw = new IssueCriterionWeight();
            icw.SaveCriterionWeights(criteriaWeightsVM.UserWeights, criteriaWeightsVM.Issue.Id, GetUserIdFromClaim());
            return View(criteriaWeightsVM);
        }

        public ActionResult Evaluation(int issueId)
        {
            IssueCreating ic = new IssueCreating();
            EvaluationVM evm = new EvaluationVM();
            IssueEvaluation ie = new IssueEvaluation();
            int userId = GetUserIdFromClaim();
            evm.Issue = ic.GetIssue(issueId);
            //ToDo check viewsettings & issueOwner
            evm.AllRatings = ie.GetAllIssueRatings(issueId, userId);
            evm.UserRatings = ie.GetIssueUserRatings(issueId, userId);
            evm.Criterias = ie.GetIssueCrtieria(issueId, userId);
            evm.Alternatives = ie.GetIssueAlternatives(issueId, userId);
            evm.RatedUsers = ie.GetRatedUsersForIssue(issueId, userId);
            return View(evm);
        }

        [HttpPost]
        public ActionResult Evaluation([FromJson] EvaluationVM evaluationVM)
        {
            IssueEvaluation ie = new IssueEvaluation();
            ie.SaveUserRatings(evaluationVM.UserRatings);
            return View(evaluationVM);
        }

        public ActionResult Decision(int issueId)
        {
            DecisionVM dvm = new DecisionVM();
            IssueCreating ic = new IssueCreating();
            IssueBrAlternative iba = new IssueBrAlternative();
            IssueDecision id = new IssueDecision();
            int userId = GetUserIdFromClaim();
            dvm.AccessRight = ic.AccessRightOfUserForIssue(userId, issueId);
            dvm.Alternatives = iba.GetIssueAlternatives(issueId, userId);
            dvm.Issue = ic.GetIssue(issueId);
            dvm.OldDecisions = id.GetOldDecisions(issueId, userId);
            dvm.Decision = id.GetDecision(issueId, userId);

            return View(dvm);
        }

        [HttpPost]
        public ActionResult Decision ([FromJson] DecisionVM decisionVM)
        {
            IssueCreating ic = new IssueCreating();
            int userId = GetUserIdFromClaim();
            IssueDecision id = new IssueDecision();
            id.SaveDecision(decisionVM.Decision,userId);
            ic.NextStage(decisionVM.Issue.Id, userId);
            return RedirectToAction("Decision","Issue", new { issueId = decisionVM.Issue.Id });
        }
    }
}