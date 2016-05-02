using MApp.DA;
using MApp.DA.Repository;
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
            ProfileModel model = UserModel.DbEntityToProfilModel(UserOp.GetUser(userId));

            model.AllProperties = PropertyModel.ToModelList(PropertyOp.Properties);
            model.Properties = PropertyModel.ToModelList(PropertyOp.GetUserProperties(userId));

            return View(model);
        }

        [HttpPost]
        public ActionResult Index([FromJson] ProfileModel profileModel)
        {
            if (profileModel == null)
            {
                return View() ;
            }
            UserOp.UpdateUser(profileModel.GetUserEntity());
            List<Property> pm = PropertyModel.ToEntityList(profileModel.Properties);
            pm = pm.GroupBy(test => test.Name)
                   .Select(grp => grp.First())
                   .ToList();
            profileModel.Properties = PropertyModel.ToModelList(PropertyOp.AddUserProperties(profileModel.Id, pm));
            return View(profileModel);
        }
    }
}