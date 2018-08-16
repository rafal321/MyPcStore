using MyPcStore.Models.Data;
using MyPcStore.Models.ViewModels.Account;
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


        // get: /account/create-account ---------------------------
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // post: /account/create-account ---------------------------
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM myModel)
        {
            
            if (!ModelState.IsValid)                // check model state
            {
                return View("CreateAccount", myModel);
            }

            // check for passwords match
            if (!myModel.Password.Equals(myModel.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password do not match.");
                return View("CreateAccount", myModel);
            }

            using (Db db = new Db())
            {
                // username has to be unique
                if (db.Users.Any(y => y.Username.Equals(myModel.Username)))
                {
                    ModelState.AddModelError("", "Username " + myModel.Username + " is taken.");
                    myModel.Username = "";
                    return View("CreateAccount", myModel);
                }
                
                UserDTO userDTO = new UserDTO() // create userDTO
                {
                    FirstName = myModel.FirstName,
                    LastName = myModel.LastName,
                    EmailAddress = myModel.EmailAddress,
                    Username = myModel.Username,
                    Password = myModel.Password
                };
                
                db.Users.Add(userDTO);  // add the DTO
                
                db.SaveChanges();       // save

                
                int id = userDTO.Id;    // add to UserRolesDTO

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id, //initialize fields
                    RoleId = 2  //this is
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            TempData["SuccessMessage"] = "You are registered. You can login now."; // Create a TempData message

            // Redirect
            return Redirect("~/account/login");
        }


    }
}