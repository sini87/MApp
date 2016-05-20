using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using MApp.Middleware.Models;

namespace MApp.Web.Hubs
{
    public class CriterionHub : Hub
    {
        public void DeleteCriteria(List<int> criteria, UserShortModel user)
        {
            Clients.All.deleteCriteria(criteria, user);
        }

        public void UpdateCriteria(CriterionModel criteria, UserShortModel user)
        {
            Clients.All.updateCriteria(criteria, user);
        }
    }
}