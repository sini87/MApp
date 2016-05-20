using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using MApp.Middleware.Models;

namespace MApp.Web.Hubs
{
    public class AlternativeHub : Hub
    {
        public void DeleteAlternatives(List<int> alternatives, UserShortModel user)
        {
            Clients.All.deleteAlternatives(alternatives, user);
        }

        public void UpdateAlternatives(AlternativeModel alternatives, UserShortModel user)
        {
            Clients.All.updateAlternatives(alternatives, user);
        }
    }
}