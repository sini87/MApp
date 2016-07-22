using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using MApp.Middleware.Models;

namespace MApp.Web.Hubs
{
    /// <summary>
    /// SignalR Hub for notification of alternative changes
    /// </summary>
    public class AlternativeHub : Hub
    {
        /// <summary>
        /// notifies clients to delete some alternatives
        /// </summary>
        /// <param name="alternatives">list of alternative ids</param>
        /// <param name="user">user (as UserShortModel) who is performing this operation</param>
        public void DeleteAlternatives(List<int> alternatives, UserShortModel user)
        {
            Clients.All.deleteAlternatives(alternatives, user);
        }

        /// <summary>
        /// notifie clients to update alnternatives
        /// </summary>
        /// <param name="alternatives">list of alternatives</param>
        /// <param name="user">user (as UserShortModel) who is performing this operation</param>
        public void UpdateAlternatives(AlternativeModel alternatives, UserShortModel user, int issueId)
        {
            Clients.All.updateAlternatives(alternatives, user, issueId);
        }
    }
}