using FroggyFlop.Models.Data;
using FroggyFlop.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FroggyFlop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageVM> pageList;
            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }
            return View(pageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            if (!ModelState.IsValid) 
            {
                return View(model);
            }
            using (Db db = new Db()) 
            {
                string slug;
                
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else 
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                if (db.Pages.Any<PageDTO>(x => x.Title == model.Title.ToUpper()))
                {
                    ModelState.AddModelError("", "That title already exist");
                    return View(model);
                }
                else if (db.Pages.Any<PageDTO>(x => x.Slug == slug)) 
                {
                    ModelState.AddModelError("", "That slug already exist");
                    return View(model);
                }

                PageDTO dto = new PageDTO { 
                    Title = model.Title.ToUpper(),
                    Slug = slug, Body = model.Body,
                    HasSideBar = model.HasSideBar,
                    Sorting = 100 
                };
                db.Pages.Add(dto);
                db.SaveChanges();
            }
            TempData["SM"] = "You have added new page";
            return RedirectToAction("Index");
        }


        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            PageVM model;
            using (Db db = new Db()) 
            {
                PageDTO dto = db.Pages.Find(id);

                if (dto == null) 
                {
                    return Content("The page does not exist");
                }

                model = new PageVM(dto);
            }

            return View(model);
        }

        // POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            if (!ModelState.IsValid) 
            {
                return View(model);
            }

            using (Db db = new Db()) 
            {
                int id = model.Id;
                string slug = null;

                PageDTO dto = db.Pages.Find(id);

                dto.Title = model.Title.ToUpper();

                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == dto.Title)) 
                {
                    ModelState.AddModelError("", "That title already exist");
                    return View(model);
                }
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That slug already exist");
                    return View(model);
                }

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;

                db.SaveChanges();
            }
            TempData["SM"] = "You have edited the page";
            return RedirectToAction("Index");
        }

        // GET: Admin/Pages/PageDetails/id
        [HttpGet]
        public ActionResult PageDetails(int id)
        {
            PageVM model;

            using (Db db = new Db()) 
            {
                PageDTO dto = db.Pages.Find(id);

                if (dto == null) 
                {
                    return Content("This page does not exist");
                }
                model = new PageVM(dto);
            }
            
            return View(model);
        }

        // GET: Admin/Pages/DeletePage/id
        [HttpGet]
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db()) 
            {
                PageDTO dto = db.Pages.Find(id);

                if (dto == null) 
                {
                    Content("This page does not exist");
                }
                db.Pages.Remove(dto);

                db.SaveChanges();
            }
            TempData["SM"] = "You have deleted a page";

            return RedirectToAction("Index");
        }

        // POST: Admin/Pages/ReorderPage
        [HttpPost]
        public void ReorderPage(int [] ids)
        {
            using (Db db = new Db()) 
            {
                int count = 1;
                PageDTO dto;
                foreach (var pageId in ids) 
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }
        }

        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            SidebarVM model;

            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1); //TODO 
                if (dto == null)
                {
                    return Content("this sidebar does not exist");
                }
                model = new SidebarVM(dto);
            }

            return View(model);

        }

        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1);

                dto.Body = model.Body;

                db.SaveChanges();
            }
            TempData["SM"] = "You have edited a siddebar";

            return RedirectToAction("EditSidebar");

        }

    }
}