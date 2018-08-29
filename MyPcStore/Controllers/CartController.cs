using MyPcStore.Models.Data;
using MyPcStore.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace MyPcStore.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            // cart list - initialize
                    //The ?? operator is called the null - coalescing operator
                    //It's used to define a default value for nullable value types or reference types
                    //It returns the left-hand operand if the operand is not null
                    //Otherwise it returns the right operand

            var myCart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // check cart for empty
            if (myCart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty.";
                return View();
            }

            decimal total = 0m;         // calculate total and save to ViewBag

            foreach (var item in myCart)
            {
                total += item.Total;
            }
            ViewBag.GrandTotal = total; //return
            
            return View(myCart);
        }

        public ActionResult CartPartial()
        {
            CartVM myModel = new CartVM();    // Initialize CartVM
            
            int qty = 0;          // initial quantity
            
            decimal price = 0m;     // initial price

            if (Session["cart"] != null)    // check for cart session
            {
                var list = (List<CartVM>)Session["cart"];   // get total qty and price

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                myModel.Quantity = qty;
                myModel.Price = price;
            }
            else
            {
                myModel.Quantity = 0;   // or set qty and price to 0
                myModel.Price = 0m;
            }
            return PartialView(myModel);
        }
        //------------------------------------------------------------
        public ActionResult AddToCartPartial(int id)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            CartVM myModel = new CartVM();

            using (Db db = new Db())
            {
                // get product
                ProductDTO myProduct = db.Products.Find(id);

                // veryfy if product is already in cart
                var productInCart = cart.FirstOrDefault(y => y.ProductId == id);

                
                if (productInCart == null)  // if not, add new
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = myProduct.Id,
                        ProductName = myProduct.Name,
                        Quantity = 1,
                        Price = myProduct.Price,
                        Image = myProduct.ImageName
                    });
                }
                else
                {
                    productInCart.Quantity++;        // increment
                }
            }
            // Get total:
            //qty; price and add to model
            int qty = 0;        ///set to zero
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            myModel.Quantity = qty;
            myModel.Price = price;
            
            Session["cart"] = cart; // save cart back to session

            // Return partial view with model
            return PartialView(myModel);
        }

        // GET: /Cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            
            List<CartVM> cart = Session["cart"] as List<CartVM>; //this is

            using (Db db = new Db())
            {
                // gget cartVM from list
                CartVM myModel = cart.FirstOrDefault(y => y.ProductId == productId);

                // increment quantity
                myModel.Quantity++;
                
                var result = new { qty = myModel.Quantity, price = myModel.Price };// store  data
                
                return Json(result, JsonRequestBehavior.AllowGet);// return json with all data
            }

        }

        // GET: /Cart/DecrementProduct
        public ActionResult DecrementProduct(int productId)
        {
            
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // get model from  my list
                CartVM myModel = cart.FirstOrDefault(y => y.ProductId == productId);

                // decrement quantity
                if (myModel.Quantity > 1)
                {
                    myModel.Quantity--;
                }
                else
                {
                    myModel.Quantity = 0;
                    cart.Remove(myModel);
                }

                // store  data
                var result = new { qty = myModel.Quantity, price = myModel.Price };

                
                return Json(result, JsonRequestBehavior.AllowGet);  // jason return
            }

        }

        // GET: /Cart/RemoveProduct
        public void RemoveProduct(int productId)
        {
            
            List<CartVM> cart = Session["cart"] as List<CartVM>; // initialize cart list

            using (Db db = new Db())
            {
                // list - get model
                CartVM myModel = cart.FirstOrDefault(y => y.ProductId == productId);

                // list - remove model 
                cart.Remove(myModel);
            }
        }

        public ActionResult PaypalPartial()     // Increment and decrement product in cart details 
        {                                       //view so they work with paypal form
            List<CartVM> theCart = Session["cart"] as List<CartVM>;

            return PartialView(theCart);
        }


        // post: /Cart/PlaceOrder
        [HttpPost]
        public void PlaceOrder()
        {
            // Get cart list
            List<CartVM> myCart = Session["cart"] as List<CartVM>;

            // Get username
            string user_name = User.Identity.Name;

            int orderId = 0;

            using (Db db = new Db())
            {
                
                OrderDTO orderDTO = new OrderDTO(); // initialize OrderDTO

                
                var q = db.Users.FirstOrDefault(y => y.Username == user_name);
                int userId = q.Id;                      // get user id

                
                orderDTO.UserId = userId;               // Add to OrderDTO and save
                orderDTO.CreatedAt = DateTime.Now;      //date

                db.Orders.Add(orderDTO);
                db.SaveChanges();
                
                orderId = orderDTO.OrderId; // Get id
                
                OrderDetailsDTO orderDetails_DTO = new OrderDetailsDTO(); // initialize
                
                foreach (var item in myCart) // add to Order Details DTO
                {
                    orderDetails_DTO.OrderId = orderId;
                    orderDetails_DTO.UserId = userId;
                    orderDetails_DTO.ProductId = item.ProductId;
                    orderDetails_DTO.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetails_DTO);

                    db.SaveChanges();
                }
            }

            //#############################################################################################
            // SEND CONFIRMATION EMAIL  
            //var client = new SmtpClient("poczta.wp.pl") // Email admin             Doesn't work!!!!
            //{
            //    Credentials = new NetworkCredential("therafk", ""),
            //    EnableSsl = true
            //};
            //client.Send("therafk@wp.pl", "therafk@gmail", "New Order", "New order made: " + orderId);
            //#############################################################################################


            Session["cart"] = null; // reseting session
        }

    }
}