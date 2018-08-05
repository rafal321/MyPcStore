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

        // get; /shop/category/name
        public ActionResult Category(string name)
        {
            
            List<ProductVM> prodVMList; // declare a list

            using (Db db = new Db())
            {
                // category id
                CategoryDTO categoryDTO = db.Categories.Where(z => z.Slug == name).FirstOrDefault();
                int catId = categoryDTO.Id;

                // initialize the list
                prodVMList = db.Products.ToArray().Where(z => z.CategoryId == catId).Select(z => new ProductVM(z)).ToList();

                // category name
                var prodCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();
                ViewBag.CategoryName = prodCat.CategoryName;
            }
            return View(prodVMList);
        }
    }
}