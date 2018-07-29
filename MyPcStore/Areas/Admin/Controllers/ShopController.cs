using MyPcStore.Models.Data;
using MyPcStore.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyPcStore.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Caregories
        public ActionResult Categories()
        {
            //return all the rows from category table in this method
            //Declare a list of models
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {
                //initialize view models    
                categoryVMList = db.Categories
                    .ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(y => new CategoryVM(y)) //need constructor here Alt+f12
                    .ToList();
            }
            
            return View(categoryVMList);
        }
    }
}