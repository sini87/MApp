using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using MApp.Middleware.Models;

namespace MApp.Web.Hubs
{
    /// <summary>
    /// SignalR HUB for notification of alternative changes
    /// </summary>
    public class CriterionHub : Hub
    {
        /// <summary>
        /// notifies clients to delete some criteria
        /// </summary>
        /// <param name="criteria">list of criteria ids</param>
        /// <param name="user">user who is performing this operation</param>
        public void DeleteCriteria(List<int> criteria, UserShortModel user)
        {
            Clients.All.deleteCriteria(criteria, user);
        }

        /// <summary>
        /// notifie clients to update criteria
        /// </summary>
        /// <param name="criteria">list of criteria </param>
        /// <param name="user">user who is perform</param>
        public void UpdateCriteria(CriterionModel criteria, UserShortModel user)
        {
            Clients.All.updateCriteria(criteria, user);
        }
    }
}