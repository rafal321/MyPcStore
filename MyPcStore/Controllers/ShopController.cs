using MyPcStore.Models.Data;
using MyPcStore.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
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

        // GET /shop/product-details/name
        [ActionName("product-details")] //name is category name
        public ActionResult ProductDetails(string name)
        {                       
            ProductVM myModel;
            ProductDTO dto;

            int id = 0;// initialize product id

            using (Db db = new Db())
            {
                // Check if product exists
                if (!db.Products.Any(y => y.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                // Initialize productDTO
                dto = db.Products.Where(y => y.Slug == name).FirstOrDefault();

                
                id = dto.Id;    // Get id

                // Init model
                myModel = new ProductVM(dto);
            }

            // get gallery images.
            myModel.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                                .Select(fn => Path.GetFileName(fn));

            
            return View("ProductDetails", myModel); // return view with model
        }

    }
}