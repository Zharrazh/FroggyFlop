using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Froggy_flop.Models
{
    public class DBInitializer : DropCreateDatabaseAlways<ShopContext>
    {
        protected override void Seed(ShopContext context)
        {
            context.Frogs.Add(new Frog { Name = "Обычная лягушка", Price = 30, ShortDesc = "Ничем не примечательная лягушка" });
            context.Frogs.Add(new Frog { Name = "Тропическая лягушка", Price = 30, ShortDesc = "Очень яркая и милая лягушка" });
            context.Frogs.Add(new Frog { Name = "Грязевая лягушка", Price = 30, ShortDesc = "Живет в грязи и ей это нравится" });
            base.Seed(context);
        }
    }
}