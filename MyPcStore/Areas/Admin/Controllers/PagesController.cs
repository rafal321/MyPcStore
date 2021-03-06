﻿using MyPcStore.Models.Abstract;
using MyPcStore.Models.Data;
using MyPcStore.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyPcStore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        private ITestTDbSet db = new Db();
        public PagesController() { }
        public PagesController(ITestTDbSet context)
        {
            db = context;
        }

        //private Db db = new Db();   //for unit testing


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
        //-----------------------------------------------------------
        //GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        //++++++++++++++++++++++++++++++++++++++++modified++++++++++++++++++++++++++++++++++++++
        //POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM myModel)
        {


            //Check Model State - has to be done after submitting the form
            if (!ModelState.IsValid)
            {
                return View(myModel);
            }


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
          
            //Redirect
            //TempData persists after redirection
            TempData["SuccessMessage"] = "New Page has been added successfully.";
            return RedirectToAction("AddPage");

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        //+++++++++++++++++++++++++++++++++++++++orginal+++++++++++++++++++++++++++++++++++++++
        ////POST: Admin/Pages/AddPage
        //[HttpPost]
        //public ActionResult AddPage(PageVM myModel)
        //{
        //    //Check Model State - has to be done after submitting the form
        //    if (! ModelState.IsValid)
        //    {
        //        return View(myModel);
        //    }

        //    using (Db db = new Db())          //commented here for unitTesting
        //    {
        //        //Declare the slug
        //        string slug;

        //        //initialize pageDTO
        //        PageDTO dto = new PageDTO();

        //        //DTO title
        //        //acces value whats in form when submitted myModel.Title from View AddPage.cshtml
        //        //      ...or(model => model.Title, new {...
        //        dto.Title = myModel.Title;

        //        //Check for Slug
        //        if (string.IsNullOrWhiteSpace(myModel.Slug))
        //        {
        //            slug = myModel.Title.Replace(" ", "- ").ToLower();
        //        }
        //        else
        //        {
        //            slug = myModel.Slug.Replace(" ", "- ").ToLower();
        //        }

        //        //Veryfy if title and slug are not the same
        //        //if it's the match that's a problem - check for that
        //        if (db.Pages.Any(y => y.Title == myModel.Title) || db.Pages.Any(z => z.Slug == slug))
        //        {
        //            // https:  //exceptionnotfound.net/asp-net-mvc-demystified-modelstate/
        //            ModelState.AddModelError("", "This title and/or slug already exists.");
        //            return View(myModel);
        //        }

        //        //DTO 
        //        dto.Slug = slug;
        //        dto.Body = myModel.Body;
        //        dto.HasSidebar = myModel.HasSidebar;
        //        //when you add the page, it's gonna be the last page

        //        dto.Sorting = 100;
        //        //DTO Save
        //        db.Pages.Add(dto);
        //        db.SaveChanges();
        //    }
        //    //Redirect
        //    //TempData persists after redirection
        //    TempData["SuccessMessage"] = "New Page has been added successfully.";
        //    return RedirectToAction("AddPage");

        //}
        //-----------------------------------------------------------
        //GET: Admin/Pages/EditPage/id
        [HttpGet]
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

        //POST: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //check model state
            if (! ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //get page id
                int id = model.Id;
                //Declare slug
                string slug = "home";
                //get the page from the table
                PageDTO dto = db.Pages.Find(id);
                //DTO title
                dto.Title = model.Title;
                //check for slug if it is not home 
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }
                //make sure title and slug are unique
                if (db.Pages.Where(y => y.Id != id).Any(z => z.Title == model.Title) ||
                    db.Pages.Where(y => y.Id != id).Any(z => z.Slug == slug))
                {
                    ModelState.AddModelError("", "Title and/or slug already exists.");
                    return View(model);
                }

                //DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                //Save DTO
                db.SaveChanges();
            }


            //Save TempData message
            TempData["SuccessMessage"] = "You have edited the page.";

            //Redirect
            return RedirectToAction("EditPage");
            
        }
        //--------------------------------------------mvc TAB TAB ---
        //GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            //Declare PageVM
            PageVM model;

            //get the page
            using (Db db = new Db())
            {
                //confirm the page is there
                PageDTO dto = db.Pages.Find(id);
                
                if (dto == null)
                {
                    return Content("The page does not exists!");
                }

                //initialize model pageVM
                model = new PageVM(dto);
                //Return view with model
            }
            return View(model);
        }
        //-----------------------------------------------------------
        //GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                //get the page
                PageDTO dto = db.Pages.Find(id);
                //remove the page
                db.Pages.Remove(dto);
                //save changes 
                db.SaveChanges();
            }
            //redirection
            return RedirectToAction("Index");
        }
        //-----------------------------------------------------------
        //POST: Admin/Pages/ReorderPages
        //has to be httpPost - 'cause default is GET - for DragAnd Drop
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db =new Db())
            {
                //set initial count
                int count = 1;

                //declare pageDTO
                PageDTO dto;

                //set sorting for each page
                foreach (var item in id)
                {
                    dto = db.Pages.Find(item);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
        }
        //-----------------------------------------------------------
        //GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            //Declare mdel
            SidebarVM model;
            using (Db db = new Db())
            {
                //get DTO
                //1 hardcoded as it's always gonna be 1
                SidebarDTO dto = db.Sidebar.Find(1);
                //Return ViewModel
                model = new SidebarVM(dto);
            }
            return View(model);
        }
        //POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                //get DTO
                SidebarDTO dto = db.Sidebar.Find(1);
                dto.Body = model.Body;
                db.SaveChanges();
            }
            TempData["SuccessMessage"] = "Sidebar has been edited.";
            return RedirectToAction("EditSidebar");
        }


    }
}