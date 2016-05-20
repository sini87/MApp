﻿using MApp.Middleware;
using MApp.Middleware.Models;
using MApp.Web.CustomLibraries;
using MApp.Web.Hubs;
using MApp.Web.Models;
using MApp.Web.ViewModel;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        private string GetUserNameFromClaim()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            string username = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            return username;
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
            vm.UserId = userId;

            if (issueId != -1)
            {
                vm.Issue = ic.GetIssue(issueId);
                vm.AccessRights = ic.GetAccessRightsOfIssue(issueId);
                AccessRightModel arm = ic.AccessRightOfUserForIssue(userId, issueId);
                vm.AccessRight = arm.Right;
                vm.SelfAssessmentDescription = arm.SelfAssessmentDescr;
                vm.SelfAssessmentValue = Convert.ToInt32(arm.SelfAssessmentValue);
                vm.Comments = ic.GetIssueComments(issueId, userId);
                vm.GroupthinkNotifications = ic.GetGroupthinkNotifications(issueId, userId);
            }
            else
            {
                vm.Issue = new IssueModel();
                vm.Issue.Status = "CREATING";
                vm.Issue.Setting = "A";
                vm.Issue.AnonymousPosting = false;
                vm.AccessRights = new List<AccessRightModel>();
                vm.AccessRights.Add(new AccessRightModel(userId, "Owner",vm.AllUsers.Where(x => x.Id == userId).FirstOrDefault().Name));
                vm.AccessRight = "O";
                vm.Issue.Id = -1;
                vm.Comments = new List<CommentModel>();
                vm.GroupthinkNotifications = new List<NotificationModel>();
            }
            vm.AllUsers = vm.AllUsers.Where(x => x.Id != userId).ToList();

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
            creatingVM.Issue.Id = ic.SaveIssue(creatingVM.Issue, userId, creatingVM.SelfAssessmentValue, creatingVM.SelfAssessmentDescription);
            issueId = creatingVM.Issue.Id;
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
        public ActionResult NextStage (int issueId, string status)
        {
            IssueCreating ic = new IssueCreating();
            ic.NextStage(issueId,GetUserIdFromClaim());
            string st = status.Remove(status.Length - 1, 1).Remove(0, 1);
            if (st == "CREATING")
            {
                return RedirectToAction("BrCriteria", "Issue", new { issueId = issueId });
            }else if (st == "BRAINSTORMING1")
            {
                return RedirectToAction("CriteriaRating", "Issue", new { issueId = issueId });
            }
            else if (st == "BRAINSTORMING2")
            {
                return RedirectToAction("EVALUATION", "Issue", new { issueId = issueId });
            }
            else if (st == "EVALUATING")
            {
                return RedirectToAction("Decision", "Issue", new { issueId = issueId });
            }
            else
            {
                return RedirectToAction("Issue", "Index", new { issueId = issueId });
            }
            
        }

        [HttpGet]
        public ActionResult BrCriteria(int issueId)
        {
            IssueCreating ic = new IssueCreating();
            BrCriteriaVM viewModel = new BrCriteriaVM();
            int userId = GetUserIdFromClaim();
            viewModel.Issue = ic.GetIssue(issueId);
            IssueBrCriteria ibc = new IssueBrCriteria();
            viewModel.IssueCriteria = ibc.GetIssueCriteria(issueId, userId);
            viewModel.AccessRight = ic.AccessRightOfUserForIssue(userId, issueId).Right;
            viewModel.UserId = userId;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult BrCriteria ([FromJson]BrCriteriaVM brCriteriaVM)
        {
            IssueBrCriteria ibc = new IssueBrCriteria();
            ibc.UpdateCriteria(brCriteriaVM.IssueCriteria, brCriteriaVM.DeletedCriteria, GetUserIdFromClaim());
            brCriteriaVM.IssueCriteria = ibc.GetIssueCriteria(brCriteriaVM.Issue.Id, GetUserIdFromClaim());

            UserShortModel user = new UserShortModel(brCriteriaVM.UserId, GetUserNameFromClaim());           
            var context = GlobalHost.ConnectionManager.GetHubContext<CriterionHub>();            
            context.Clients.All.updateCriteria(brCriteriaVM.IssueCriteria, user);
            if (brCriteriaVM.DeletedCriteria != null && brCriteriaVM.DeletedCriteria.Count > 0)
            {
                context.Clients.All.deleteCriteria(brCriteriaVM.DeletedCriteria, user);
            }

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
            vm.AccessRight = ic.AccessRightOfUserForIssue(userId, issueId).Right;
            vm.UserId = userId;
            return View(vm);
        }

        [HttpPost]
        public ActionResult BrAlternatives([FromJson] BrAlternativesVM brAlternativesVM)
        {
            IssueBrAlternative iba = new IssueBrAlternative();
            int userId = GetUserIdFromClaim();
            iba.UpdateAlternatives(brAlternativesVM.Alternatives, brAlternativesVM.DeletedAlternatives, userId);
            brAlternativesVM.Alternatives = iba.GetIssueAlternatives(brAlternativesVM.Issue.Id, userId);

            UserShortModel user = new UserShortModel(brAlternativesVM.UserId, GetUserNameFromClaim());
            var context = GlobalHost.ConnectionManager.GetHubContext<AlternativeHub>();
            context.Clients.All.updateAlternatives(brAlternativesVM.Alternatives, user);
            if (brAlternativesVM.DeletedAlternatives != null && brAlternativesVM.DeletedAlternatives.Count > 0)
            {
                context.Clients.All.deleteAlternatives(brAlternativesVM.DeletedAlternatives, user);
            }

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
            vm.AccessRight = ic.AccessRightOfUserForIssue(userId, issueId).Right;
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

            evm.AccessRight = ic.AccessRightOfUserForIssue(userId, issueId).Right;
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
            dvm.AccessRight = ic.AccessRightOfUserForIssue(userId, issueId).Right;
            dvm.Alternatives = iba.GetIssueAlternatives(issueId, userId);
            dvm.Issue = ic.GetIssue(issueId);
            dvm.OldDecisions = id.GetOldDecisions(issueId, userId);
            dvm.Decision = id.GetDecision(issueId, userId);

            return View(dvm);
        }

        [HttpPost]
        public ActionResult Decision ([FromJson] DecisionModel decisionModel)
        {
            IssueCreating ic = new IssueCreating();
            int userId = GetUserIdFromClaim();
            IssueDecision id = new IssueDecision();
            id.SaveDecision(decisionModel,userId);
            ic.NextStage(decisionModel.IssueId, userId);
            return RedirectToAction("Decision","Issue", new { issueId = decisionModel.IssueId });
        }

        public ActionResult UpdateDecision([FromJson] DecisionModel decisionModel)
        {
            IssueDecision id = new IssueDecision();
            id.UpdateDecision(decisionModel,GetUserIdFromClaim());
            return RedirectToAction("Decision", "Issue", new { issueId = decisionModel.IssueId });
        }

        [HttpPost]
        public HttpResponseMessage AddComment(CommentModel commentModel)
        {

            IssueCreating ic = new IssueCreating();
            ic.AddCommentToAlternative(commentModel, GetUserIdFromClaim());

            commentModel.UserId = GetUserIdFromClaim();
            commentModel.Name = GetUserNameFromClaim();
            var context = GlobalHost.ConnectionManager.GetHubContext<CommentHub>();
            context.Clients.All.addNewComment(commentModel);

            return new HttpResponseMessage();
        }

        [HttpPost]
        public HttpResponseMessage AddNotification(NotificationModel notificationModel)
        {
            IssueCreating ic = new IssueCreating();
            notificationModel.UserId = GetUserIdFromClaim();
            ic.MakeNotification(notificationModel);
            return new HttpResponseMessage();
        }

        [HttpPost]
        public HttpResponseMessage MarkNotificationAsRead(int notificationId)
        {
            IssueCreating ic = new IssueCreating();
            ic.MarkNotificationAsRead(notificationId);
            return new HttpResponseMessage();
        }
    }
}