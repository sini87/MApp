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
    }
}