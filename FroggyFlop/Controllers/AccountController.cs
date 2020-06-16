using FroggyFlop.Models.Data;
using FroggyFlop.Models.ViewModels.Account;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FroggyFlop.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: Account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            if (!ModelState.IsValid)
                return View("CreateAccount", model);
            if(model.Password!=model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Password do not match!");
                return View("CreateAccount", model);
            }
            using (Db db = new Db())
            { 
                if(db.Users.Any(x=>x.Username ==model.Username))
                {
                    ModelState.AddModelError("", $"Username {model.Username} is taken");
                    return View("CreateAccount", model);
                }
                UserDTO dto = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Username = model.Username,
                    Password = model.Password
                };
                db.Users.Add(dto);
                db.SaveChanges();

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = dto.Id,
                    RoleId = 2
                };
                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }
            TempData["SM"] = "You are now registered and can login";
            return RedirectToAction("Login");
        }

        // GET: Account/login
        [HttpGet]
        public ActionResult Login()
        {
            if(!string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("user-profile");
            }
            return View();
        }

        // POST: Account/login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username == model.Username && x.Password == model.Password))
                    isValid = true;

            }
            if (!isValid)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }
            FormsAuthentication.SetAuthCookie(model.Username, model.IsRemember);
            return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.IsRemember));
        }

        // GET: Account/logout
        [Authorize]
        [HttpGet]
        public ActionResult Logout(LoginUserVM model)
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        // GET: Account/UserNavPartial
        [HttpGet]
        [Authorize]
        public ActionResult UserNavPartial()
        {
            string userName = User.Identity.Name;

            UserNavPartialVM model;

            using (Db db = new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);
                if(dto==null)
                {
                    FormsAuthentication.SignOut();
                    return PartialView(null);
                }

                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }

            return PartialView(model);

        }

        // GET: Account/user-profile
        [HttpGet]
        [ActionName ("user-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {
            string userName = User.Identity.Name;

            UserProfileVM model;

            using (Db db = new Db())
            {
                UserDTO dto= db.Users.FirstOrDefault(x => x.Username == userName);
                model = new UserProfileVM(dto);
            }

            return View("UserProfile", model);
        }

        // GET: Account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile(UserProfileVM model)
        {
            if (!ModelState.IsValid)
                return View("UserProfile", model);
            bool passwordEdited = false;
            if(!string.IsNullOrEmpty(model.Password))
            {
                if(model.Password!=model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match");
                    return View("UserProfile", model);
                }
                passwordEdited = true;

            }

            UserDTO dto;
            using (Db db = new Db())
            {
                string userName = User.Identity.Name;

                if (db.Users.Where(x => x.Id != model.Id).Any(x=>x.Username == userName))
                {
                    ModelState.AddModelError("", $"Username {model.Username} already exist");
                    return View("UserProfile", model);
                }
                dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.Email = model.Email;
                dto.Username = model.Username;
                if (passwordEdited)
                    dto.Password = model.Password;

                db.SaveChanges();
            }

            TempData["SM"] = "You have edited your profile";
            if (User.Identity.Name != dto.Username)
                return RedirectToAction("Logout");
            return View("UserProfile", model);
        }
    }
}