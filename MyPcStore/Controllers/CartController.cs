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
            return View();
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