using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Froggy_flop.Models;

namespace Froggy_flop.Controllers
{
    public class HomeController : Controller
    {
        ShopContext frogContext = new ShopContext();
        public ActionResult Index()
        {
            //извлекаем данные из датблицы frogs
            IEnumerable<Frog> frogs = frogContext.Frogs;

            ViewBag.Frogs = frogs;

            return View();
        }

        [HttpGet]
        public ActionResult Buy(int id) {
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        public string Buy(Purchase purchase) {
            purchase.DateTime = DateTime.Now;

            frogContext.Purchases.Add(purchase);

            frogContext.SaveChanges();

            return $"Уважаемый {purchase.FIO}, скоро с вами не свяжутся потому что магазин не работает";
        }
    }
}