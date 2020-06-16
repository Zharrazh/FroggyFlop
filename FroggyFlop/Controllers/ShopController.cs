using FroggyFlop.Models.Data;
using FroggyFlop.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FroggyFlop.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        // GET: Shop
        public ActionResult CategoryMenuPartial()
        {
            List<CategoryVM> categoryList;

            using (Db db = new Db())
            {
                categoryList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
            return PartialView("_CategoryMenuPartial", categoryList);
        }

        // GET: Shop/Category/name
        public ActionResult Category(string name)
        {
            List<ProductVM> productList;

            using (Db db = new Db())
            {
                CategoryDTO categoryDTO = db.Categories.Where(x=>x.Slug ==name).FirstOrDefault();
                if (categoryDTO == null)
                    return Content("This category does not exist");

                productList = db.Products.ToArray().Where(x => x.CategoryId == categoryDTO.Id).Select(x => new ProductVM(x)).ToList();

                ViewBag.CategoryName = categoryDTO.Name;
            }
            return View(productList);
        }
        //GET: Shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            ProductVM model;

            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.ToArray().Where(x => x.Slug == name).FirstOrDefault();
                if (dto == null)
                    return Content("This product does not exist");
                model = new ProductVM(dto);
            }

            
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath($"~/Images/Uploads/Products/{model.Id}/Gallery/Thumbs"))
                .Select(file=>Path.GetFileName(file));

                
            return View("ProductDetails",model);
        }


    }
}