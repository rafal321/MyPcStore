using MyPcStore.Models.Data;
using MyPcStore.Models.ViewModels.Account;
using MyPcStore.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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

        // post: /account/login     --- LOGIN ---
        [HttpPost]
        public ActionResult Login(LoginUserVM myModel)
        {
            
            if (!ModelState.IsValid)    // this is
            {
                return View(myModel);
            }

            bool isUserValid = false;   // check if the user is valid

            using (Db db = new Db())
            {
                if (db.Users.Any(y => y.Username.Equals(myModel.Username) && y.Password.Equals(myModel.Password)))
                {
                    isUserValid = true;
                }
            }

            if (!isUserValid)
            {
                ModelState.AddModelError("", "Invalid password or username.");
                return View(myModel);
            }
            else
            {   //save cookie or session for user
                FormsAuthentication.SetAuthCookie(myModel.Username, myModel.RememberMe);
                //redirect to whtever was before login page
                return Redirect(FormsAuthentication.GetRedirectUrl(myModel.Username, myModel.RememberMe));
            }
        }

        // get: /account/Logout     --- LOGOUT ---
        [Authorize] //only AUTHORIZE that means all the roles are allowed 
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/account/login");
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

        [Authorize]
        public ActionResult UserNavPartial()
        {
            string userNam = User.Identity.Name;// get username

            UserNavPartialVM modelPartial;

            using (Db db = new Db())
            {
                // get user
                UserDTO userdto = db.Users.FirstOrDefault(y => y.Username == userNam);
                
                modelPartial = new UserNavPartialVM() // build the model
                {
                    FirstName = userdto.FirstName,
                    LastName = userdto.LastName
                };
            }
            return PartialView(modelPartial); // return partial view with model
        }

        // get : /account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {
            string userName = User.Identity.Name; // get username

            UserProfileVM myModel;

            using (Db db = new Db())
            {
                // get theuser
                UserDTO dto = db.Users.FirstOrDefault(y => y.Username == userName);

                // build  the model
                myModel = new UserProfileVM(dto);
            }

            // Return view with model
            return View("UserProfile", myModel);
        }


        // post: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile(UserProfileVM myModel)
        {
            
            if (!ModelState.IsValid)        // heck for model state
            {
                return View("UserProfile", myModel);
            }

            // passwords match or not 
            if (!string.IsNullOrWhiteSpace(myModel.Password))
            {
                if (!myModel.Password.Equals(myModel.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Your password does not match.");
                    return View("UserProfile", myModel);
                }
            }

            using (Db db = new Db())
            {
                string username = User.Identity.Name; // get username

                // Make sure username is unique
                if (db.Users.Where(y => y.Id != myModel.Id).Any(y => y.Username == username))
                {
                    ModelState.AddModelError("", "Username " + myModel.Username + " already exists.");
                    myModel.Username = "";
                    return View("UserProfile", myModel);
                }

                UserDTO dto = db.Users.Find(myModel.Id); // dto edit myModel.ID

                dto.FirstName = myModel.FirstName;
                dto.LastName = myModel.LastName;
                dto.EmailAddress = myModel.EmailAddress;
                dto.Username = myModel.Username;

                if (!string.IsNullOrWhiteSpace(myModel.Password))
                {
                    dto.Password = myModel.Password;
                }
                db.SaveChanges();
            }

            TempData["SuccessMessage"] = "You have edited your profile!";
            return Redirect("~/account/user-profile"); //redirection
        }

        // get: /account/Orders
        [Authorize(Roles = "User")] //accesible only to users
        public ActionResult Orders()
        {
            List<OrdersForUserVM> ordersOverviewForUser = new List<OrdersForUserVM>(); // initialize list of OrdersForUserVM

            using (Db db = new Db())
            {
                UserDTO user = db.Users.Where(y => y.Username == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;           // get the user id

                List<OrderVM> orders = db.Orders.Where(y => y.UserId == userId).ToArray().Select(y => new OrderVM(y)).ToList();
                
                foreach (var order in orders) // loop through list of order view models
                {
                    // Init products dict
                    Dictionary<string, int> productsAndQTY = new Dictionary<string, int>();
                    
                    decimal total = 0m; // declare total
                                                            // initialize list of Order Details dto
                    List<OrderDetailsDTO> orderDetailsDTO = db.OrderDetails.Where(z => z.OrderId == order.OrderId).ToList();

                    foreach (var orderDetails in orderDetailsDTO)   // loop though list of Order Details dto
                    {
                        ProductDTO product = db.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();
                        
                        decimal price = product.Price; // get product price
                        
                        string productName = product.Name; // get product name
                        
                        productsAndQTY.Add(productName, orderDetails.Quantity); // add to products dict
                        
                        total += orderDetails.Quantity * price; // get total
                    }
                    
                    ordersOverviewForUser.Add(new OrdersForUserVM() // add to orders for usser list
                    {
                        OrderNumber = order.OrderId,
                        Total = total,
                        ProductsAndQty = productsAndQTY,
                        CreatedAt = order.CreatedAt
                    });
                }

            }

            return View(ordersOverviewForUser); // return view with list of orders for users dto
        }
    }
}