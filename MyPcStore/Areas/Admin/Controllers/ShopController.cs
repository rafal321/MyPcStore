using MyPcStore.Models.Data;
using MyPcStore.Models.ViewModels.Shop;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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

        //POST: Admin/shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            ProductVM myModel = new ProductVM();
            using (Db db = new Db())
            {
                myModel.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            return View(myModel);
        }

        //POST: Admin/shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM myModel, HttpPostedFileBase file)
        {
            // Check model VMstate
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    myModel.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(myModel);
                }
            }

            // uniquiness of product
            using (Db db = new Db())
            {
                if (db.Products.Any(y => y.Name == myModel.Name))
                {
                    myModel.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "This product name is already taken!");
                    return View(myModel);
                }
            }

            
            int id; // product id declared

            // Init and save productDTO
            using (Db db = new Db())
            {
                ProductDTO product = new ProductDTO();

                product.Name = myModel.Name;
                product.Slug = myModel.Name.Replace(" ", "-").ToLower();
                product.Description = myModel.Description;
                product.Price = myModel.Price;
                product.CategoryId = myModel.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(y => y.Id == myModel.CategoryId);
                product.CategoryName = catDTO.Name;     //categoryId in productVM is a foreign key
                db.Products.Add(product);              //in categories table
                db.SaveChanges();

                
                id = product.Id; // Get the id - is PK of just inserted  with ProductDTO product = new ProductDTO();
            }

            // Set TempData message
            TempData["SuccessMessage"] = "You have added a product.";

            #region Upload Image

            // create directories
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\"))); //specyfy

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString()); //specific product
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs"); //main image thumb
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery"); //gallery immages
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs"); //x

            if (!Directory.Exists(pathString1)) //has to check if directory exists
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))//has to check if directory exists
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))//has to check if directory exists
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))//has to check if directory exists
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))//has to check if directory exists
                Directory.CreateDirectory(pathString5);

            // doublecheck if a file was properly uploaded
            if (file != null && file.ContentLength > 0) //check
            {
                
                string ext = file.ContentType.ToLower(); // get file extension

                // Verify files extensions
                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg" &&
                    ext != "image/gif" && ext != "image/x-png" && ext != "image/png")
                {

                    //if its not then there's problem

                    using (Db db = new Db())
                    {
                        myModel.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "Image was not uploaded - unsupported type.");
                        return View(myModel);
                    }
                }


                //string imageName = file.FileName; // Initialize image name CREATES ERROR!
                string imageName = new FileInfo(file.FileName).Name; //Works in Edge/Chrome/FF

                // ssave image name to DTO
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // sset original and thumb image paths
                var path = string.Format("{0}\\{1}", pathString2, imageName); //thumb
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);//image paths

                // Save original
                file.SaveAs(path);

                // create and save thumbnails
                WebImage img = new WebImage(file.InputStream); //set
                img.Resize(180, 180);
                img.Save(path2);
            }

            #endregion

            
            return RedirectToAction("AddProduct");// redirection
        }

        // gGET: Admin/Shop/Products
        public ActionResult Products(int? page, int? catId)
        {
            //https:  //github.com/troygoode/PagedList
            // declare a list ProductVM

            List<ProductVM> myListOfProductVM;

            // page number - setting
            var pageNumber = page ?? 1;

            using (Db db = new Db())
            {
                // Initialize the list
                myListOfProductVM = db.Products.ToArray().Where(y => catId == null || catId == 0 || y.CategoryId == catId)
                                  .Select(y => new ProductVM(y)).ToList();

                // populate categories select list - for only one select of categories
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // for setting selected category
                ViewBag.SelectedCat = catId.ToString();
            }

            // pagination setup
            var onePageOfProducts = myListOfProductVM.ToPagedList(pageNumber, 3); //num of pages low, so i can see
            ViewBag.OnePageOfProducts = onePageOfProducts;

            
            return View(myListOfProductVM);
        }

        // GET: Admin/Shop/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            // Declare product vie model
            ProductVM myModel;

            using (Db db = new Db())
            {
                
                ProductDTO dto = db.Products.Find(id);

                // check if product exists
                if (dto == null)
                {
                    return Content("This product does not exist.");
                }

                // init model
                myModel = new ProductVM(dto);

                // make a selecedt list
                myModel.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // gallery images comes from IEnumerable<string> GaleryImages ..... from ProductVM
                myModel.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                                .Select(fileName => Path.GetFileName(fileName));
                //returns list of IEnumerable of files
            }
            return View(myModel);
        }

        // POST: Admin/Shop/EditProduct/id
        [HttpPost]
        public ActionResult EditProduct(ProductVM myModel, HttpPostedFileBase file)
        {
            // Get product id
            int id = myModel.Id;

            // opulate categories select list and gallery of images
            //so I dont have to do it each time
            using (Db db = new Db())
            {
                myModel.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            myModel.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                                .Select(fileName => Path.GetFileName(fileName));

            // check current model state
            if (!ModelState.IsValid)
            {
                return View(myModel);
            }

            // check if product name is unique
            using (Db db = new Db())
            {
                if (db.Products.Where(y => y.Id != id).Any(y => y.Name == myModel.Name))
                {
                    ModelState.AddModelError("", "This product name is taken!");
                    return View(myModel);
                }
            }

            // update the product
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);

                dto.Name = myModel.Name;
                dto.Slug = myModel.Name.Replace(" ", "-").ToLower();
                dto.Description = myModel.Description;
                dto.Price = myModel.Price;
                dto.CategoryId = myModel.CategoryId;
                dto.ImageName = myModel.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(y => y.Id == myModel.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }

            // Set TempData message
            TempData["SuccessMessage"] = "You have edited the product!";

            #region Image Upload

            // file upload check
            if (file != null && file.ContentLength > 0)
            {

                // extensions
                string ext = file.ContentType.ToLower();

                // extension verification
                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg" && 
                    ext != "image/gif" && ext != "image/x-png" && ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "Image was not uploaded - unsupported type.");
                        return View(myModel);
                    }
                }

                // directory paths setup for upload
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                // files deletion from directories

                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in di1.GetFiles())
                    file2.Delete();

                foreach (FileInfo file3 in di2.GetFiles())
                    file3.Delete();

                // save name of image
                //string imageName = file.FileName; // Initialize image name CREATES ERROR!
                string imageName = new FileInfo(file.FileName).Name; //Works in Edge/Chrome/FF


                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // ssave original and thumb images
                var path = string.Format("{0}\\{1}", pathString1, imageName);
                var path2 = string.Format("{0}\\{1}", pathString2, imageName);

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(180, 180);
                img.Save(path2);
            }

            #endregion

            return RedirectToAction("EditProduct");
        }


    }
}