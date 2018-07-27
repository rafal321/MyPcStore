using MyPcStore.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyPcStore.Models.ViewModels.Pages
{
    //page View Model
    //data annotations when form is submitted
    public class PageVM
    {
        //-wyklad7-create constructor that is taking DTO as parameter
        public PageVM()
        {
        }
        public PageVM(PageDTO currentrow)
        {
            Id = currentrow.Id;
            Title = currentrow.Title;
            Slug = currentrow.Slug;
            Body = currentrow.Body;
            Sorting = currentrow.Sorting;
            HasSidebar = currentrow.HasSidebar;
        }


        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength =3)]
        public string Title { get; set; }
        public string Slug { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        [AllowHtml]         //chapter 17 - AllowHtml
        public string Body { get; set; }
        public int Sorting { get; set; }
        public bool HasSidebar { get; set; }
    }
}