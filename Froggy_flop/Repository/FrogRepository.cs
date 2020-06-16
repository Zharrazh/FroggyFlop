using Froggy_flop.Interfaces;
using Froggy_flop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Froggy_flop.Repository
{
    public class FrogRepository : IFrogs
    {
        private readonly ShopContext shopContext;
        public FrogRepository(ShopContext shopContext)
        {
            this.shopContext = shopContext;
        }

        public IEnumerable<Frog> All()
        {
            return from frog in shopContext.Frogs select frog;
        }

        public Frog getFrogById(int id)
        {
            return (from frog in shopContext.Frogs where frog.Id=id select frog).FirstOrDefault();
        }
    }
}