using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using MApp.Middleware.Models;

namespace MApp.Web.Hubs
{
    public class CommentHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void AddNewComment(CommentModel comment)
        {
            Clients.All.addNewComment(comment);
        }
    }
}