using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyPcStore.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return Redirect("~/account/login");
        }
        
        [HttpGet]
        public ActionResult Login()             // GET: /account/login
        {
            string userName = User.Identity.Name;   // make sure user is not logged in
                                                    //Identity created by system
            if (!string.IsNullOrEmpty(userName))
                return RedirectToAction("user-profile");
            
            return View(); // return view
        }


        // get: /account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

    }
}