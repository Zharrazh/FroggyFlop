using FroggyFlop.Models.Data;
using FroggyFlop.Models.ViewModels.Shop;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebGrease.Extensions;

namespace FroggyFlop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        [HttpGet]
        public ActionResult Categories()
        {
            List<CategoryVM> categoryList;

            using (Db db = new Db())
            {
                categoryList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
            return View(categoryList);
        }

        // GET: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            string id;

            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";
                CategoryDTO dto = new CategoryDTO
                {
                    Name = catName,
                    Slug = catName.Replace(" ", "-").ToLower(),
                    Sorting = 100
                };
                db.Categories.Add(dto);
                db.SaveChanges();
                id = dto.Id.ToString();
            }

            return id;
        }

        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {

            using (Db db = new Db())
            {
                int count = 1;
                CategoryDTO dto;
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }
        }

        // POST: Admin/Shop/DeleteCategory/id
        [HttpGet]
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                CategoryDTO dto = db.Categories.Find(id);
                if (dto == null)
                    return Content("This category does not exist");
                db.Categories.Remove(dto);

                db.SaveChanges();
            }
            TempData["SM"] = "You have deleted a category";

            return RedirectToAction("Categories");

        }

        // POST: Admin/Shop/DeleteCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == newCatName))
                {
                    return "titletaken";
                }

                CategoryDTO dto = db.Categories.Find(id);
                if (dto != null)
                {
                    dto.Name = newCatName;
                    dto.Slug = newCatName.Replace(" ", "-").ToLower();
                }

                db.SaveChanges();
            }

            return "ok";

        }

        // GET: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            ProductVM model = new ProductVM();

            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            return View(model);
        }

        // POST: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                }

                return View(model);
            }

            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "This product name is taken");
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            int id;

            using (Db db = new Db())
            {
                ProductDTO dto = new ProductDTO();

                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                CategoryDTO categoryDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = categoryDTO.Name ?? "";

                db.Products.Add(dto);
                db.SaveChanges();
                id = dto.Id;
            }
            TempData["SM"] = "You have added a product";
            #region Upload Image
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            string[] paths = new string[] {
                 Path.Combine(originalDirectory.ToString(), "Products"),
                 Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString()),
                 Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString()+ "\\Thumbs"),
                 Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery"),
                 Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs")
            };
            foreach (var path in paths)
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }

            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension");
                    }
                    return View(model);
                }


                string imageName = file.FileName;

                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }

                string[] pathWithFiles = new string[] {
                    string.Format($"{paths[1]}\\{imageName}"),
                    string.Format($"{paths[2]}\\{imageName}")
                };

                file.SaveAs(pathWithFiles[0]);

                WebImage wi = new WebImage(file.InputStream);
                wi.Resize(200, 200).Crop(1,1);
                wi.Save(pathWithFiles[1]);
            }
            #endregion

            
            return RedirectToAction("AddProduct");
        }

        // GET: Admin/Shop/Products
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            List<ProductVM> productList;

            int pageNumber = page ?? 1;

            using (Db db = new Db())
            {
                productList = db.Products.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x=>new ProductVM(x)).ToList();

                ViewBag.Categories = new SelectList(db.Categories.ToList(),"Id", "Name");

                ViewBag.SelectedCat = catId.ToString();

            }

            var onePageOfProducts = productList.ToPagedList(pageNumber,3);
            ViewBag.onePageOfProducts = onePageOfProducts;

            return View(productList);
        }


        // GET: Admin/Shop/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            ProductVM model;

            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);

                if (dto == null) 
                {
                    return Content("That product does not exist");
                }

                model = new ProductVM(dto);

                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            }

            return View(model);
        }
        // POST: Admin/Shop/EditProduct/id
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            int id = model.Id;
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs"))
                   .Select(fn => Path.GetFileName(fn));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                if(db.Products.Where(x=> x.Id!=id).Any(x=>x.Name==model.Name))
                {
                    ModelState.AddModelError("", "That product name is taken");
                    return View(model);
                }
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                ProductDTO dto = db.Products.Find(id);
                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDto = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDto.Name;

                db.SaveChanges();
            }

            TempData["SM"] = "You have edited the product";

            #region Image Upload

            if(file!=null&&file.ContentLength>0)
            {
                string ext = file.ContentType.ToLower();
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension");
                    }
                    return View(model);
                }

                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                string[] paths = new string[] {
                 Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString()),
                 Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString()+ "\\Thumbs")
                };
                foreach (var path in paths)
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    foreach(var files in di.GetFiles())
                    {
                        files.Delete();
                    }
                }


                string imageName = file.FileName;

                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }

                string[] pathWithFiles = new string[] {
                    string.Format($"{paths[0]}\\{imageName}"),
                    string.Format($"{paths[1]}\\{imageName}")
                };

                file.SaveAs(pathWithFiles[0]);

                WebImage wi = new WebImage(file.InputStream);
                wi.Resize(200, 200).Crop(1, 1);
                wi.Save(pathWithFiles[1]);

            }

            #endregion

            return RedirectToAction("EditProduct");


        }

        // GET: Admin/Shop/DeleteProducts/id
        [HttpGet]
        public ActionResult DeleteProduct(int id)
        {
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                if (dto == null)
                {
                    return Content("That product does not exist");
                }
                db.Products.Remove(dto);
                db.SaveChanges();
            }

            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            string path = Path.Combine(originalDirectory.ToString(), "Products\\"+id);

            if (Directory.Exists(path))
            {
                Directory.Delete(path,true);
            }

            return RedirectToAction("Products");

        }

        // GET: Admin/Shop/SaveGalleryImages
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];

                if(file!=null&&file.ContentLength>0)
                {
                    var originalDirectory = new DirectoryInfo($"{Server.MapPath(@"\")}Images\\Uploads");
                    string mainPath = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString()+"\\Gallery");
                    string thubsPath = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    string path1 = $"{mainPath}\\{file.FileName}";
                    string path2 = $"{thubsPath}\\{file.FileName}";

                    file.SaveAs(path1);

                    WebImage wi = new WebImage(file.InputStream);
                    wi.Resize(200, 200).Crop(1, 1);
                    wi.Save(path2);
                }
            }
        }

        // POST: Admin/Shop/DeleteImage?id=id&imageName=imageName
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString()+ "/Gallery/"+ imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);

            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);

        }

    }
    
}