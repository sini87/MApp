using MApp.Middleware;
using MApp.Middleware.Models;
using MApp.Web.CustomLibraries;
using MApp.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MApp.Web.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.SerialNumber).Value);
            Authentication auth = new Authentication();

            return View(auth.GetUserProfileModel(userId));
        }

        [HttpPost]
        public ActionResult Index([FromJson] ProfileModel profileModel)
        {
            if (profileModel == null)
            {
                return View() ;
            }
            Authentication auth = new Authentication();
            return View(auth.UpdateProfile(profileModel));
        }
    }
}