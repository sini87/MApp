using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using MApp.Middleware.Models;
using MApp.Web.ViewModel;

namespace MApp.Web.Hubs
{
    /// <summary>
    /// This is the main SignalR hub used for notifications
    /// </summary>
    public class NotificationHub : Hub
    {
        /// <summary>
        /// notification when some criteria weights are updated
        /// </summary>
        /// <param name="weights">list of updated criteria weights</param>
        /// <param name="user">user who is performing this operation</param>
        public void UpdateCriteriaWeights(List<CriterionWeightModel> weights, UserShortModel user)
        {
            Clients.All.updateCriteriaWeights(weights, user);
        }

        /// <summary>
        /// notification when user updates/makes alternative evaluation
        /// </summary>
        /// <param name="userRatings">list of user ratings</param>
        /// <param name="user">user who is performing this operation</param>
        public void UpdateEvaluation(List<RatingModel> userRatings, UserShortModel user)
        {
            Clients.All.updateRatings(userRatings, user);
        }

        /// <summary>
        /// notification when contributor or viewer sends a groupshift notification
        /// </summary>
        /// <param name="notification">the notification</param>
        public void SendNotification(NotificationModel notification)
        {
            Clients.All.sendNotification(notification);
        }

        /// <summary>
        /// notification when an user is added to issue
        /// </summary>
        /// <param name="issue">issue to which user is added</param>
        /// <param name="accessRights">access right information</param>
        /// <param name="user">user who is performing this operation</param>
        public void UserAddedToIssue(IssueModel issue, List<AccessRightModel> accessRights, int userId)
        {
            Clients.All.userAddedToIssue(issue, accessRights, userId);
        }

        /// <summary>
        /// notification when some user is removed from issue
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="removedUserId">id of the removed user</param>
        /// <param name="userId">user who is performing this operation</param>
        public void UserRemovedFromIssue(int issueId, int removedUserId, int userId)
        {
            Clients.All.userRemovedFromIssue(issueId, removedUserId, userId);
        }

        /// <summary>
        /// notifies clients when issue core information were updated
        /// only used on Creating.cshtml/issue main site
        /// </summary>
        /// <param name="issue">IssueModel</param>
        /// <param name="addedTags">list of new tags</param>
        /// <param name="removedTags">list of removed tags</param>
        /// <param name="userId">user who is performing this operation</param>
        public void UpdateIssue(IssueModel issue, List<TagModel> addedTags, List<TagModel> removedTags, List<TagModel> issueTags, int userId, int selfAssessmentValue, string selfAssessmendDescr)
        {
            Clients.All.updateIssue(issue, addedTags, removedTags, issueTags, userId, selfAssessmentValue, selfAssessmendDescr);
        }

        /// <summary>
        /// notifies clients to refresh activity index
        /// use only on Issue main page (Creating.cshtml)
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        public void UpdateActivity(int issueId, int userId)
        {
            Clients.All.updateActivity(issueId, userId);
        }

        /// <summary>
        /// notifies clients when issue is put to next stage
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="status">new issue status</param>
        /// <param name="userId">user who is performing this operation</param>
        public void NextStage(int issueId, string status, int userId)
        {
            Clients.All.nextStage(issueId, status, userId);
        }

        /// <summary>
        /// notifies clients when a decision is made or updated
        /// </summary>
        /// <param name="decisionVM">decsiion view model</param>
        /// <param name="changedByUserId">user who made the tecision</param>
        public void DecisionUpdated(DecisionVM decisionVM, int changedByUserId)
        {
            Clients.All.decisionUpdated(decisionVM, changedByUserId);
        }

        /// <summary>
        /// notifies clients when an issue review is made
        /// </summary>
        /// <param name="reviewModel">issue review model</param>
        public void ReviewSaved(ReviewModel reviewModel)
        {
            Clients.All.reivewSaved(reviewModel);
        }
    }
}