using MyPcStore.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
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


    }
}