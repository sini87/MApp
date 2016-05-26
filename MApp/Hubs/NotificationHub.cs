using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using MApp.Middleware.Models;

namespace MApp.Web.Hubs
{
    public class NotificationHub : Hub
    {
        public void UpdateCriteriaWeights(List<CriterionWeightModel> weights, UserShortModel user)
        {
            Clients.All.updateCriteriaWeights(weights, user);
        }

        public void UpdateEvaluation(List<RatingModel> userRatings, UserShortModel user)
        {
            Clients.All.updateRatings(userRatings, user);
        }

        public void SendNotification(NotificationModel notification)
        {
            Clients.All.sendNotification(notification);
        }

        public void UserAddedToIssue(IssueModel issue, List<AccessRightModel> accessRights, int userId)
        {
            Clients.All.userAddedToIssue(issue, accessRights, userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="removedUserId">id of the removed user</param>
        /// <param name="userId">user who is performing this operation</param>
        public void UserRemovedFromIssue(int issueId, int removedUserId, int userId)
        {
            Clients.All.userRemovedFromIssue(issueId, removedUserId, userId);
        }

        /// <summary>
        /// notifys the update of issue core infos
        /// only used on Creating.cshtml/issue main site
        /// </summary>
        /// <param name="issue"></param>
        /// <param name="addedTags"></param>
        /// <param name="removedTags"></param>
        /// <param name="userId"></param>
        public void UpdateIssue(IssueModel issue, List<TagModel> addedTags, List<TagModel> removedTags, List<TagModel> issueTags, int userId, int selfAssessmentValue, string selfAssessmendDescr)
        {
            Clients.All.updateIssue(issue, addedTags, removedTags, issueTags, userId, selfAssessmentValue, selfAssessmendDescr);
        }

        /// <summary>
        /// notifies clients to refresh activity index
        /// use only on Issue main page (Creating.cshtml)
        /// </summary>
        /// <param name="issueId"></param>
        public void UpdateActivity(int issueId, int userId)
        {
            Clients.All.updateActivity(issueId, userId);
        }
    }
}