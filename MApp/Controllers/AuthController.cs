using MApp.DA.Repository;
using MApp.Middleware;
using MApp.Middleware.Models;
using MApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MApp.Web.Controllers
{
    /// <summary>
    /// MVC Controller class for login, logout and register new User
    /// </summary>
    [AllowAnonymous]
    public class AuthController : Controller
    {
        /// <summary>
        /// returns login view (Auth/index.cshtml)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Action with post request to perform login
        /// </summary>
        /// <param name="model">Login model with eMail & password</param>
        /// <returns>if login successful then user will be redirected to issues ovewview, else returning to login view with LoginModel error </returns>
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid) // checks if input vields have the correct format
            {
                return View(model); //returns the view with the input so that the user doesnt have to retype it again
            }

            Authentication auth = new Authentication();
            UserModel user = auth.Login(model.Email, model.Password);

            if (user != null)
            {
                var identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                        new Claim(ClaimTypes.SerialNumber, user.Id.ToString())
                    }, "ApplicationCookie");

                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;

                authManager.SignIn(identity);

                return RedirectToAction("Index", "Issue");
            }

            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        /// <summary>
        /// action to logout user
        /// </summary>
        /// <returns>redirection to Login view</returns>
        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login", "Auth");
        }

        /// <summary>
        /// Action to Registration view
        /// </summary>
        /// <returns>registration view</returns>
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        /// <summary>
        /// mvc post request to register User
        /// </summary>
        /// <param name="model">user model with required fields</param>
        /// <returns>if registration is successfull user will be logged in an redirected to issues overview else return will be registration view with model errors</returns>
        [HttpPost]
        public ActionResult Registration(UserModel model)
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