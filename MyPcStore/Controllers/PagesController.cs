using MyPcStore.Models.Data;
using MyPcStore.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyPcStore.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page} it's optional page
        public ActionResult Index(string page = "") //optional page parameter
        {
            
            if (page == "")     // Get/set page slug
                page = "home";  //this page is the slug

            
            PageVM myModel;     // Declare model and View Model
            PageDTO dto;        // Declare model and DTO

            // Check if page exists
            using (Db db = new Db())
            {
                if (!db.Pages.Any(y => y.Slug.Equals(page)))   //if no match its a problem
                {
                    return RedirectToAction("Index", new { page = "" }); //rederect to home page
                }
            }

            // get page data transfer object
            using (Db db = new Db())
            {
                dto = db.Pages.Where(y => y.Slug == page).FirstOrDefault();
            }                           //FirstOrDefault() if there is no match it returns null

            
            ViewBag.PageTitle = dto.Title;  // set the page title

            
            if (dto.HasSidebar == true)     // check if there's sidebar
            {
                ViewBag.Sidebar = "Yes";        //sidebar is bollean 
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            myModel = new PageVM(dto);    // initialize model
            return View(myModel);         // return view with model
        }


        public ActionResult PagesMenuPartial()
        {
            
            List<PageVM> pagesVMList; // Declare a list of PageVM

            using (Db db = new Db())// get all pages except the home
            {                       //ascending order is default
                pagesVMList = db.Pages.ToArray().OrderBy(y => y.Sorting).Where(y => y.Slug != "home").Select(y => new PageVM(y)).ToList();
            }
            
            return PartialView(pagesVMList); // return partial view with the list
        }

        public ActionResult SidebarPartial()
        {
            
            SidebarVM myModel; //model

            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebar.Find(1);

                myModel = new SidebarVM(dto);
            }

            return PartialView(myModel);
        }

    }
}