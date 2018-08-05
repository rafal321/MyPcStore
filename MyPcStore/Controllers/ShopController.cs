using MyPcStore.Models.Data;
using MyPcStore.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyPcStore.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages"); //index method/pages controller
        }

        public ActionResult CategoryMenuPartial()
        {
            
            List<CategoryVM> catVMList; // declare list CategoryVM

            
            using (Db db = new Db()) // initialize the list
            {
                catVMList = db.Categories.ToArray().OrderBy(y => y.Sorting).Select(y => new CategoryVM(y)).ToList();
            }
            
            return PartialView(catVMList);// return partial with list
        }
    }
}