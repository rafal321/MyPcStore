using MyPcStore.Models.Data;
using MyPcStore.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyPcStore.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //list of page view models
            //Declare list of Page View Models
            List<PageVM> pageslist;

            //initialize list - using statement for connection closing
            using (Db db = new Db())
            {                                                        //need empty constructor here Alt+f12
                pageslist = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //return view with list
            return View(pageslist);
        }

        //GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        //POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM myModel)
        {
            //Check Model State - has to be done after submitting the form
            if (! ModelState.IsValid)
            {
                return View(myModel);
            }

            using (Db db = new Db())
            {
                //Declare the slug
                string slug;

                //initialize pageDTO
                PageDTO dto = new PageDTO();

                //DTO title
                //acces value whats in form when submitted myModel.Title from View AddPage.cshtml
                //      ...or(model => model.Title, new {...
                dto.Title = myModel.Title;

                //Check for Slug
                if (string.IsNullOrWhiteSpace(myModel.Slug))
                {
                    slug = myModel.Title.Replace(" ", "- ").ToLower();
                }
                else
                {
                    slug = myModel.Slug.Replace(" ", "- ").ToLower();
                }

                //Veryfy if title and slug are not the same
                //if it's the match that's a problem - check for that
                if (db.Pages.Any(y => y.Title == myModel.Title) || db.Pages.Any(z => z.Slug == slug))
                {
                    // https:  //exceptionnotfound.net/asp-net-mvc-demystified-modelstate/
                    ModelState.AddModelError("", "This title and/or slug already exists.");
                    return View(myModel);
                }

                //DTO 
                dto.Slug = slug;
                dto.Body = myModel.Body;
                dto.HasSidebar = myModel.HasSidebar;
                //when you add the page, it's gonna be the last page

                dto.Sorting = 100;
                //DTO Save
                db.Pages.Add(dto);
                db.SaveChanges();
            }
            //Redirect
            TempData["SuccessMessage"] = "New Page has been added successfully.";
            return RedirectToAction("AddPage");

        }

        //GET: Admin/Pages/AddPage/id
        public ActionResult EditPage(int id)
        {
            //ActionResult as the return type you can return 
            //view, redirect or partial view. Or content(string) etc  



            //declare page VM (view model)
            PageVM model;
            using (Db db = new Db())
            {
                //get page
                //will select the row with PK of id value
                PageDTO dto = db.Pages.Find(id);
                //confirm page exists
                if (dto == null)
                {
                    return Content("Page does not exists.");
                }
                //initialize page VM

                model = new PageVM(dto);
                //I use constructor from the class
                //othervise I would have to initialize all fields here

            }

            //return view with the model
            return View(model);
        }
    }
}