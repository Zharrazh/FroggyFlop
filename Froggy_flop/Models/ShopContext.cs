using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Froggy_flop.Models
{
    public class ShopContext : DbContext
    {
        public DbSet<Frog> Frogs { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}