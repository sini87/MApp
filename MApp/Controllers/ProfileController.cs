using MApp.DA;
using MApp.DA.Repository;
using MApp.Web.CustomLibraries;
using MApp.Web.Models;
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
            ProfileModel model = UserModel.DbEntityToProfilModel(UserOp.GetUser(userId));

            List<PropertyModel> propModelList = new List<PropertyModel>();

            foreach (Property p in PropertyOp.Properties)
            {
                propModelList.Add(PropertyModel.FromEntity(p));
            }
            model.Properties = propModelList;
            ViewData["Name"] = model.Name;

            return View(model);
        }

        [HttpPost]
        public ActionResult Index([FromJson] ProfileModel profileModel)
        {
            UserOp.UpdateUser(profileModel.GetUserEntity());
            ViewData["Name"] = profileModel.Name;
            return View(profileModel);
        }
    }
}