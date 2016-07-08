using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using MApp.Middleware.Models;

namespace MApp.Web.Hubs
{
    /// <summary>
    /// SignalR hub for add new comment notofication
    /// </summary>
    public class CommentHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        /// <summary>
        /// notifies clients that comment is added
        /// </summary>
        /// <param name="comment">new comment as CommentModel</param>
        public void AddNewComment(CommentModel comment)
        {
            Clients.All.addNewComment(comment);
        }
    }
}