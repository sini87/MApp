using MApp.DA;
using MApp.DA.Repository;
using MApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MApp.Web.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid) // checks if input vields have the correct format
            {
                return View(model); //returns the view with the input so that the user doesnt have to retype it again
            }

            MApp.DA.User user = UserOp.Login(model.Email, model.Password);

            if (user != null)
            {
                var identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                        new Claim(ClaimTypes.SerialNumber, user.Id.ToString())
                    }, "ApplicationCookie");

                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;

                authManager.SignIn(identity);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                if (UserOp.Register(model.Email, model.FirstName, model.LastName, model.Password, model.SecretQuestion, model.Answer, model.StakeholderDescription))
                {
                    LoginModel lm = new LoginModel();
                    lm.Email = model.Email;
                    lm.Password = model.Password;
                    return Login(lm);
                }
            }
            ModelState.AddModelError("", "Email already exists");
            return View(model);
        }
    }
}