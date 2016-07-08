using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
[assembly: OwinStartup(typeof(MApp.Web.App_Start.Startup))]
namespace MApp.Web.App_Start
{
    /// <summary>
    /// startup class for configuring the application
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //sets coockie authentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/auth/login")
            });
            //sets up signalR to application
            app.MapSignalR();
        }
    }
}
    