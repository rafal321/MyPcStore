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
            List<CategoryVM> categoryVMList; //declare

            using (Db db = new Db())
            {
                //initialize view models    
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting)
                    .Select(y => new CategoryVM(y)).ToList(); //need constructor here Alt+f12
            }
            return View(categoryVMList);
        }

        // POSTT: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName) //called by JS in Categories.cshtml
        {
            string id;          //declare id
            using (Db db = new Db())
            {
                if (db.Categories.Any(y => y.Name == catName)) //check unique category
                {
                    return "titletaken";
                }
                CategoryDTO dto = new CategoryDTO(); //initialize dto
                dto.Name = catName;//add to dto
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;  //same as the pages, newly category is the last one
                db.Categories.Add(dto);
                db.SaveChanges();
                id = dto.Id.ToString(); //need to get Id here!
            }
            return id;
        }

        //POST: Admin/shop/ReorderCategories
        //has to be httpPost - 'cause default is GET
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                //set initial count
                int count = 1;

                //declare categoryDTO
                CategoryDTO dto;

                //set sorting for each categoryy
                foreach (var item in id)
                {
                    dto = db.Categories.Find(item);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
        }

        //GET: Admin/shop/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                //get the category
                CategoryDTO dto = db.Categories.Find(id);
                //remove the category
                db.Categories.Remove(dto);
                //save changes 
                db.SaveChanges();
            }
            //redirection
            return RedirectToAction("Categories");
        }

        //POST: Admin/shop/RenameCategory
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                //is category unique?
                if (db.Categories.Any(y => y.Name == newCatName))
                {
                    return "titletaken";
                }
                CategoryDTO dto = db.Categories.Find(id);
                // edit this DTO
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();
                db.SaveChanges();
            }
            return "something";
        }
    }
}