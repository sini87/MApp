using MApp.Middleware;
using MApp.Middleware.Models;
using MApp.Web.CustomLibraries;
using MApp.Web.Hubs;
using MApp.Web.Models;
using MApp.Web.ViewModel;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
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
            KeyValuePair<int, List<UserIssueModel>> kvp = new KeyValuePair<int, List<UserIssueModel>>(userId, iO.GetUIM(userId));
            return View(kvp);
        }


        public ActionResult Creating(int issueId)
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
                vm.GroupshiftProperties = ic.GetGropshiftProperties(issueId);
                if (ic.MarkAsRead(issueId, userId))
                {
                    var ctx2 = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    ctx2.Clients.All.updateActivity(issueId, userId);
                }
                vm.UserWithMostChanges = ic.UserWithMostChanges(issueId);
                if (vm.AccessRight == "O")
                {
                    vm.AllUserChangeCounts = ic.GetAllChangeCountsByUser(issueId);
                    vm.GroupActivity = ic.GetGroupActivity(issueId);
                    vm.GroupTrustworthiness = ic.GetGroupTrustworthiness(issueId);
                    vm.DecisionTrustworthiness = ic.GetDecisionTrustworthiness(issueId);
                }
                else
                {
                    vm.AllUserChangeCounts = new List<KeyValuePair<UserShortModel, int>>();
                    vm.GroupActivity = new List<KeyValuePair<string, int>>();
                    vm.GroupTrustworthiness = new List<string>();
                    vm.DecisionTrustworthiness = new List<string>();
                }
                vm.UserChangesCount = ic.GetUserChangesCount(issueId, userId);
                vm.InfoCount = ic.GetInfoCountForUser(issueId, userId);
                vm.ReadInfoCount = ic.GetReadInfoCountForUser(issueId, userId);
                vm.UnreadInformation = ic.GetUnreadInformation(issueId, userId);
                vm.UserChanges = ic.GetUserChanges(issueId, userId);
                vm.LastChange = ic.GetLastChange(issueId);
                vm.Last100Changes = ic.GetLast100Changes(issueId);
            }
            else
            {
                vm.Issue = new IssueModel();
                vm.Issue.Status = "CREATING";
                vm.Issue.Setting = "A";
                vm.Issue.AnonymousPosting = false;
                vm.AccessRights = new List<AccessRightModel>();
                vm.AccessRights.Add(new AccessRightModel(userId, "Owner", vm.AllUsers.Where(x => x.Id == userId).FirstOrDefault().Name));
                vm.AccessRight = "O";
                vm.Issue.Id = -1;
                vm.Comments = new List<CommentModel>();
                vm.GroupthinkNotifications = new List<NotificationModel>();
                vm.GroupshiftProperties = new List<KeyValuePair<string, List<string>>>();
                vm.InfoCount = 0;
                vm.ReadInfoCount = 0;
                vm.UserChangesCount = 0;
                vm.UserChanges = new List<UserChangeModel>();
                vm.UnreadInformation = new List<KeyValuePair<string, int>>();
                vm.Last100Changes = new List<UserChangeModel>();
                vm.LastChange = new UserChangeModel();
                vm.GroupActivity = new List<KeyValuePair<string, int>>();
                vm.GroupTrustworthiness = new List<string>();
                vm.DecisionTrustworthiness = new List<string>();
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
            int oldId = creatingVM.Issue.Id;
            int userId = GetUserIdFromClaim();
            creatingVM.Issue.Id = ic.SaveIssue(creatingVM.Issue, userId, creatingVM.SelfAssessmentValue, creatingVM.SelfAssessmentDescription);
            issueId = creatingVM.Issue.Id;
            ic.UpdateIsseuTags(creatingVM.Issue.Id, creatingVM.AddedTags, creatingVM.DeletedTags, userId);
            ic.UpdateAccessRights(creatingVM.AddedAR, creatingVM.DeletedAR, creatingVM.AccessRights, issueId, userId);

            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            if (oldId < 0)
            {
                context.Clients.All.userAddedToIssue(creatingVM.Issue, creatingVM.AccessRights, userId);
            }
            context.Clients.All.updateIssue(creatingVM.Issue, creatingVM.AddedTags, creatingVM.DeletedTags, ic.GetIssueTags(issueId), userId, creatingVM.SelfAssessmentValue, creatingVM.SelfAssessmentDescription);
            context.Clients.All.updateActivity(issueId, userId);

            return RedirectToAction("Creating", "Issue", new { issueId = creatingVM.Issue.Id });
        }

        [HttpPost]
        public ActionResult DeleteIssue(int issueId)
        {
            IssueCreating ic = new IssueCreating();
            ic.DeleteIssue(issueId);
            return RedirectToAction("Index", "Issue");
        }

        [HttpPost]
        public ActionResult NextStage(int issueId, string status)
        {
            IssueCreating ic = new IssueCreating();
            ic.NextStage(issueId, GetUserIdFromClaim());
            string st = status.Remove(status.Length - 1, 1).Remove(0, 1);
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            int userId = GetUserIdFromClaim();

            if (st == "CREATING")
            {
                context.Clients.All.nextStage(issueId, "BRAINSTORMING1", userId);
                return RedirectToAction("BrCriteria", "Issue", new { issueId = issueId });
            } else if (st == "BRAINSTORMING1")
            {
                context.Clients.All.nextStage(issueId, "BRAINSTORMING2", userId);
                return RedirectToAction("CriteriaRating", "Issue", new { issueId = issueId });
            }
            else if (st == "BRAINSTORMING2")
            {
                context.Clients.All.nextStage(issueId, "EVALUATING", userId);
                return RedirectToAction("EVALUATION", "Issue", new { issueId = issueId });
            }
            else if (st == "EVALUATING")
            {
                context.Clients.All.nextStage(issueId, "DECIDING", userId);
                return RedirectToAction("Decision", "Issue", new { issueId = issueId });
            }
            else
            {
                context.Clients.All.nextStage(issueId, "FINISHED", userId);
                return RedirectToAction("Index", "Issue");
            }

        }

        [HttpGet]
        public ActionResult BrCriteria(int issueId)
        {
            IssueCreating ic = new IssueCreating();
            BrCriteriaVM viewModel = new BrCriteriaVM();
            int userId = GetUserIdFromClaim();
            viewModel.Issue = ic.GetIssue(issueId);
            if (viewModel.Issue.Status == "CREATING")
            {
                return RedirectToAction("Creating", "Issue", new { issueId = issueId });
            }
            IssueBrCriteria ibc = new IssueBrCriteria();
            viewModel.IssueCriteria = ibc.GetIssueCriteria(issueId, userId);
            viewModel.AccessRight = ic.AccessRightOfUserForIssue(userId, issueId).Right;
            viewModel.UserId = userId;
            if (ibc.MarkAsRead(issueId, userId))
            {
                var ctx2 = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                ctx2.Clients.All.updateActivity(issueId, userId);
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult BrCriteria([FromJson]BrCriteriaVM brCriteriaVM)
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
            var ctx2 = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            ctx2.Clients.All.updateActivity(brCriteriaVM.Issue.Id, brCriteriaVM.UserId);

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
            if (vm.Issue.Status == "CREATING")
            {
                return RedirectToAction("Creating", "Issue", new { issueId = issueId });
            }
            IssueBrAlternative iba = new IssueBrAlternative();
            vm.Alternatives = iba.GetIssueAlternatives(issueId, userId);
            vm.AccessRight = ic.AccessRightOfUserForIssue(userId, issueId).Right;
            vm.UserId = userId;
            if (iba.MarkAsRead(issueId, userId))
            {
                var ctx2 = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                ctx2.Clients.All.updateActivity(issueId, userId);
            }
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
            var ctx2 = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            ctx2.Clients.All.updateActivity(brAlternativesVM.Issue.Id, brAlternativesVM.UserId);
            return View(brAlternativesVM);
        }

        public ActionResult CriteriaRating(int issueId)
        {
            CriteriaWeightsVM vm = new CriteriaWeightsVM();
            IssueCreating ic = new IssueCreating();
            IssueCriterionWeight icw = new IssueCriterionWeight();
            int userId = GetUserIdFromClaim();
            vm.Issue = ic.GetIssue(issueId);
            if (vm.Issue.Status == "CREATING" || vm.Issue.Status == "BRAINSTORMING1")
            {
                return RedirectToAction("Creating", "Issue", new { issueId = issueId });
            }
            vm.AccessRight = ic.AccessRightOfUserForIssue(userId, issueId).Right;
            vm.UserId = userId;

            if (vm.Issue.Setting == "B")
            {
                vm.PCCriteria = icw.GetPCCriteria(issueId, userId);
                vm.SliderValues = icw.GetSliderValues();
            }

            vm.UserWeights = icw.GetUserWeights(issueId, userId);
            vm.OtherWeights = icw.GetIssueWeights(issueId, userId);
            vm.VotedUsers = new List<UserWithCW>();
            int i = 0;
            foreach (List<CriterionWeightModel> cwmL in vm.OtherWeights)
            {
                vm.VotedUsers.Add(new UserWithCW(cwmL.FirstOrDefault().UserId, cwmL.FirstOrDefault().Name));
                vm.VotedUsers[i].UserCriterionWeights = vm.OtherWeights[i];
                i++;
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult CriteriaRating([FromJson] CriteriaWeightsVM criteriaWeightsVM)
        {
            IssueCriterionWeight icw = new IssueCriterionWeight();
            if (criteriaWeightsVM.Issue.Setting == "A")
            {
                icw.SaveCriterionWeights(criteriaWeightsVM.UserWeights, criteriaWeightsVM.Issue.Id, GetUserIdFromClaim());

                var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                context.Clients.All.updateCriteriaWeights(criteriaWeightsVM.UserWeights, new UserShortModel(criteriaWeightsVM.UserId, GetUserNameFromClaim()));

                var ctx2 = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                ctx2.Clients.All.updateActivity(criteriaWeightsVM.Issue.Id, criteriaWeightsVM.UserId);
            } else if (criteriaWeightsVM.Issue.Setting == "B")
            {
                icw.SavePCCriteria(criteriaWeightsVM.PCCriteria, criteriaWeightsVM.UserId, GetUserNameFromClaim());
            }

            return View(criteriaWeightsVM);
        }

        public ActionResult Evaluation(int issueId)
        {
            IssueCreating ic = new IssueCreating();
            EvaluationVM evm = new EvaluationVM();
            IssueEvaluation ie = new IssueEvaluation();
            int userId = GetUserIdFromClaim();
            evm.Issue = ic.GetIssue(issueId);
            if (evm.Issue.Status == "CREATING" || evm.Issue.Status == "BRAINSTORMING1" || evm.Issue.Status == "BRAINSTORMING2")
            {
                return RedirectToAction("Creating", "Issue", new { issueId = issueId });
            }
            //ToDo check viewsettings & issueOwner
            evm.AllRatings = ie.GetAllIssueRatings(issueId, userId);
            evm.UserRatings = ie.GetIssueUserRatings(issueId, userId);
            evm.Criterias = ie.GetIssueCrtieria(issueId, userId);
            evm.Alternatives = ie.GetIssueAlternatives(issueId, userId);
            evm.RatedUsers = ie.GetRatedUsersForIssue(issueId, userId);
            evm.RatedUserCnt = evm.RatedUsers.Count;
            evm.AccessRight = ic.AccessRightOfUserForIssue(userId, issueId).Right;
            evm.UserId = userId;

            if (evm.Issue.Setting == "B")
            {
                IssueCriterionWeight icw = new IssueCriterionWeight();
                evm.SliderValues = icw.GetSliderValues();
                evm.PairwiseRatings = ie.GetPairwiseAlternativeRatings(issueId, userId);
            }

            return View(evm);
        }

        [HttpPost]
        public ActionResult Evaluation([FromJson] EvaluationVM evaluationVM)
        {
            IssueEvaluation ie = new IssueEvaluation();
            ie.SaveUserRatings(evaluationVM.UserRatings);

            List<RatingModel> userRatings = new List<RatingModel>();
            for (int i = 0; i < evaluationVM.UserRatings.Count(); i++)
            {
                foreach (RatingModel rat in evaluationVM.UserRatings[i])
                {
                    userRatings.Add(rat);
                }
            }
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.All.updateRatings(userRatings, new UserShortModel(evaluationVM.UserId, GetUserNameFromClaim()));

            var ctx2 = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            ctx2.Clients.All.updateActivity(evaluationVM.Issue.Id, evaluationVM.UserId);

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
            dvm.Alternatives = iba.GetIssueAlternatives(issueId, userId).OrderByDescending(x => x.Rating).ToList();
            dvm.Issue = ic.GetIssue(issueId);

            if (dvm.Issue.Status == "CREATING" || dvm.Issue.Status == "BRAINSTORMING1" || dvm.Issue.Status == "BRAINSTORMING2" || dvm.Issue.Status == "EVALUATING")
            {
                return RedirectToAction("Creating", "Issue", new { issueId = issueId });
            }

            dvm.OldDecisions = id.GetOldDecisions(issueId, userId);
            dvm.Decision = id.GetDecision(issueId, userId);
            dvm.UserId = GetUserIdFromClaim();
            return View(dvm);
        }

        [HttpPost]
        public ActionResult Decision(DecisionModelVM dmvm)
        {
            DecisionModel decisionModel = new DecisionModel();
            decisionModel.AlternativeId = dmvm.AlternativeId;
            decisionModel.ChangeDate = dmvm.ChangeDate;
            decisionModel.Explanation = dmvm.Explanation;
            decisionModel.IssueId = dmvm.IssueId;
            if (dmvm.Explanation == null)
            {
                return RedirectToAction("Decision", "Issue", new { issueId = decisionModel.IssueId });
            }
            IssueCreating ic = new IssueCreating();
            int userId = GetUserIdFromClaim();
            IssueDecision id = new IssueDecision();
            id.SaveDecision(decisionModel, userId);
            ic.NextStage(decisionModel.IssueId, userId);

            var ctx2 = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            ctx2.Clients.All.updateActivity(decisionModel.IssueId, userId);

            int issueId = decisionModel.IssueId;
            DecisionVM dvm = new DecisionVM();
            dvm.OldDecisions = id.GetOldDecisions(issueId, userId);
            dvm.Decision = id.GetDecision(issueId, userId);
            dvm.Issue = ic.GetIssue(issueId);
            ctx2.Clients.All.decisionUpdated(dvm, issueId);

            return RedirectToAction("Decision", "Issue", new { issueId = decisionModel.IssueId });
        }

        public ActionResult UpdateDecision( DecisionModelVM dmvm)
        {
            IssueDecision id = new IssueDecision();
            DecisionModel decisionModel = new DecisionModel();
            decisionModel.AlternativeId = dmvm.AlternativeId;
            decisionModel.ChangeDate = dmvm.ChangeDate;
            decisionModel.Explanation = dmvm.Explanation;
            decisionModel.IssueId = dmvm.IssueId;
            id.UpdateDecision(decisionModel, GetUserIdFromClaim());

            int issueId = decisionModel.IssueId;
            int userId = GetUserIdFromClaim();
            DecisionVM dvm = new DecisionVM();
            IssueCreating ic = new IssueCreating();
            dvm.OldDecisions = id.GetOldDecisions(issueId, userId);
            dvm.Decision = id.GetDecision(issueId, userId);
            dvm.Issue = ic.GetIssue(issueId);
            var ctx2 = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            ctx2.Clients.All.decisionUpdated(dvm, issueId);

            return RedirectToAction("Decision", "Issue", new { issueId = decisionModel.IssueId });
        }

        [HttpPost]
        public HttpResponseMessage AddComment(CommentModel commentModel)
        {

            IssueCreating ic = new IssueCreating();
            ic.AddCommentToAlternative(commentModel, GetUserIdFromClaim());

            commentModel.UserId = GetUserIdFromClaim();
            if (commentModel.Anonymous)
            {
                commentModel.Name = "Anonymous";
            } else
            {
                commentModel.Name = GetUserNameFromClaim();
            }
            var context = GlobalHost.ConnectionManager.GetHubContext<CommentHub>();
            context.Clients.All.addNewComment(commentModel);

            var ctx2 = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            ctx2.Clients.All.updateActivity(commentModel.IssueId, commentModel.UserId);

            return new HttpResponseMessage();
        }

        [HttpPost]
        public HttpResponseMessage AddNotification(NotificationModel notificationModel)
        {
            IssueCreating ic = new IssueCreating();
            notificationModel.UserId = GetUserIdFromClaim();
            notificationModel.Id = ic.SendNotification(notificationModel);

            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.All.sendNotification(notificationModel);

            return new HttpResponseMessage();
        }

        [HttpPost]
        public HttpResponseMessage MarkNotificationAsRead(int notificationId)
        {
            IssueCreating ic = new IssueCreating();
            ic.MarkNotificationAsRead(notificationId);
            return new HttpResponseMessage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessRight"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddAccessRight(AccessRightModel accessRight)
        {
            HttpResponseMessage msg = new HttpResponseMessage();
            IssueCreating ic = new IssueCreating();
            if (ic.AddAccessRight(accessRight, GetUserIdFromClaim()))
            {
                msg.StatusCode = System.Net.HttpStatusCode.OK;
                var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                List<AccessRightModel> l = new List<AccessRightModel>();
                l.Add(accessRight);
                context.Clients.All.userAddedToIssue(ic.GetIssue(accessRight.IssueId), l, GetUserIdFromClaim());
            }
            else
            {
                msg.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessRight"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage RemoveAccessRight(AccessRightModel accessRight)
        {
            HttpResponseMessage msg = new HttpResponseMessage();
            IssueCreating ic = new IssueCreating();
            if (ic.RemoveAccessRight(accessRight, GetUserIdFromClaim()))
            {
                msg.StatusCode = System.Net.HttpStatusCode.OK;

                var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                context.Clients.All.userRemovedFromIssue(accessRight.IssueId, accessRight.UserId, GetUserIdFromClaim());
            }
            else
            {
                msg.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return msg;
        }

        [HttpPost]
        public JsonResult GetGroupshiftProperties(int issueId)
        {
            IssueCreating ic = new IssueCreating();
            var result = new JsonResult
            {
                Data = JsonConvert.SerializeObject(ic.GetGropshiftProperties(issueId))
            };
            return result;
        }

        public HttpResponseMessage MarkCommentsAsRead(int issueId, string type)
        {
            HttpResponseMessage msg = new HttpResponseMessage();
            int userId = GetUserIdFromClaim();
            if (type == "Issue")
            {
                IssueCreating ic = new IssueCreating();
                ic.MarkCommentsAsRead(issueId, userId);
            } else if (type == "Alternative")
            {
                IssueBrAlternative iba = new IssueBrAlternative();
                iba.MarkCommentsAsRead(issueId, userId);
            }
            else if (type == "Criterion")
            {
                IssueBrCriteria ibc = new IssueBrCriteria();
                ibc.MarkCommentsAsRead(issueId, userId);
            }
            return msg;
        }

        /// <summary>
        /// returns UserIssueModel
        /// method is called when some user is on the overview page and new issue is added
        /// which he can access
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetUserIssueModel(int issueId, int userId)
        {
            IssueOverview io = new IssueOverview();
            var result = new JsonResult
            {
                Data = JsonConvert.SerializeObject(io.GetUserIssueModel(issueId, userId))
            };
            return result;
        }

        [HttpPost]
        public JsonResult SelfAssessmentRefreshed(int issueId, int userId)
        {
            IssueCreating ic = new IssueCreating();
            var result = new JsonResult
            {
                Data = JsonConvert.SerializeObject(ic.GetAccessRight(issueId, userId))
            };
            return result;
        }

        [HttpPost]
        public JsonResult RefreshActivityIndex(int issueId, int userId, string right)
        {
            IssueCreating ic = new IssueCreating();
            CreatingVM vm = new CreatingVM();

            vm.UserWithMostChanges = ic.UserWithMostChanges(issueId);
            if (right == "O")
            {
                vm.AllUserChangeCounts = ic.GetAllChangeCountsByUser(issueId);
                vm.AllUserChangeCounts = ic.GetAllChangeCountsByUser(issueId);
                vm.GroupActivity = ic.GetGroupActivity(issueId);
                vm.GroupTrustworthiness = ic.GetGroupTrustworthiness(issueId);
                vm.DecisionTrustworthiness = ic.GetDecisionTrustworthiness(issueId);
            }
            else
            {
                vm.AllUserChangeCounts = new List<KeyValuePair<UserShortModel, int>>();
                vm.AllUserChangeCounts = new List<KeyValuePair<UserShortModel, int>>();
                vm.GroupActivity = new List<KeyValuePair<string, int>>();
                vm.GroupTrustworthiness = new List<string>();
                vm.DecisionTrustworthiness = new List<string>();
            }
            vm.UserChangesCount = ic.GetUserChangesCount(issueId, userId);
            vm.InfoCount = ic.GetInfoCountForUser(issueId, userId);
            vm.LastChange = ic.GetLastChange(issueId);
            vm.Last100Changes = ic.GetLast100Changes(issueId);
            vm.UnreadInformation = ic.GetUnreadInformation(issueId, userId);
            var result = new JsonResult
            {
                Data = JsonConvert.SerializeObject(vm)
            };

            return result;
        }

        /// <summary>
        /// should be called from All issues page when notification comes in that an issue is updated
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        [HttpPost]
        public JsonResult RefreshUserIssue(int issueId, int userId)
        {
            IssueOverview io = new IssueOverview();
            UserIssueModel uim = io.GetUserIssueModel(issueId, userId);
            var result = new JsonResult
            {
                Data = JsonConvert.SerializeObject(uim)
            };
            return result;
        }

        /// <summary>
        /// trys to save pairwise criteria comparison
        /// returns false if consistencycheck faild
        /// else true
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public bool SaveCriteriaWeightsAHP(int issueId, List<PairwiseComparisonCriterionModel> list)
        {
            int userId = GetUserIdFromClaim();
            string userName = GetUserNameFromClaim();
            IssueCriterionWeight icw = new IssueCriterionWeight();
            List<CriterionWeightModel> cwList = icw.SavePCCriteria(list, userId, userName);

            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.All.updateCriteriaWeights(cwList, new UserShortModel(userId, userName));

            var ctx2 = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            ctx2.Clients.All.updateActivity(issueId, userId);

            if (cwList.Count == 0)
            {
                return false;
            }else
            {
                return true;
            }
        }

        [HttpPost]
        public string SaveAlternativeRatingAHP(int issueId, List<PairwiseComparisonRatingModel> list)
        {
            IssueEvaluation ie = new IssueEvaluation();
            string msg = ie.SaveAHPAlternativeEvaluation(list, issueId, GetUserIdFromClaim());
            int userId = GetUserIdFromClaim();
            string userName = GetUserNameFromClaim();
            List<RatingModel> ratList = ie.GetAllIssueRatings(issueId, userId).Where(x => x.UserId == userId).OrderBy(x => x.CriterionId).ThenBy(x => x.AlternativeId).ToList();

            if (msg == "success")
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                context.Clients.All.updateRatings(ratList, new UserShortModel(userId, userName));

                var ctx2 = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                ctx2.Clients.All.updateActivity(issueId, userId);
            }

            return msg;
        }

        /// <summary>
        /// should be used if somebody adds new core information (Alternative, Criterion)
        /// and some other user have currently focused the regarding page
        /// also he sees this new information instantly 
        /// this new information should be makrked as read
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="type">type coudl be "Criterion" or "Alternative"</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage MarkCoreInfoAsRead(int issueId, string type)
        {
            HttpResponseMessage msg = new HttpResponseMessage();

            if (type == "Criterion")
            {
                IssueBrCriteria ibc = new IssueBrCriteria();
                ibc.MarkAsRead(issueId, GetUserIdFromClaim());
            }else if(type == "Alternative")
            {
                IssueBrAlternative iba = new IssueBrAlternative();
                iba.MarkAsRead(issueId, GetUserIdFromClaim());
            }

            msg.StatusCode = System.Net.HttpStatusCode.OK;
            return msg;
        }

        /// <summary>
        /// should be used if somebody adds a new comment (for Issue, Alternative, Criterion)
        /// and some other user have currently focused the regarding page and expanded comments
        /// also he sees this new comment instantly 
        /// this new information should be makrked as read
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage MarkCommentAsRead(int issueId, string type)
        {
            HttpResponseMessage msg = new HttpResponseMessage();

            if (type == "Criterion")
            {
                IssueBrCriteria ibc = new IssueBrCriteria();
                ibc.MarkCommentsAsRead(issueId, GetUserIdFromClaim());
            }
            else if (type == "Alternative")
            {
                IssueBrAlternative iba = new IssueBrAlternative();
                iba.MarkCommentsAsRead(issueId, GetUserIdFromClaim());
            }else if (type == "Issue")
            {
                IssueCreating ic = new IssueCreating();
                ic.MarkCommentsAsRead(issueId, GetUserIdFromClaim());
            }

            msg.StatusCode = System.Net.HttpStatusCode.OK;
            return msg;
        }
    }
}