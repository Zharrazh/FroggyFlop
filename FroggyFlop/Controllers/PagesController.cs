using FroggyFlop.Models.Data;
using FroggyFlop.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace FroggyFlop.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            if (page == "")
                page = "home";

            PageVM model;

            PageDTO dto;
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x => x.Slug == page))
                    return RedirectToAction("Index", new { page = "" });

                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            ViewBag.PageTitle = dto.Title;

            if (dto.HasSideBar)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            model = new PageVM(dto);
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            List<PageVM> pageList;

            using (Db db = new Db()) 
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x=>new PageVM(x)).ToList();
            }

            return PartialView("_PagesMenuPartial",pageList);
        }

        public ActionResult SidebarPartial()
        {
            SidebarVM model;

            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1);
                model = new SidebarVM(dto);
            }
            

            return PartialView("_SidebarPartial", model);
        }
    }


}