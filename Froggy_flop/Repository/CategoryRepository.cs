using Froggy_flop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Froggy_flop.Repository
{
    public class CategoryRepository
    {
        private readonly ShopContext shopContext;
        public CategoryRepository(ShopContext shopContext)
        {
            this.shopContext = shopContext;
        }

        public IEnumerable<Category> All()
        {
            return from category in shopContext.Categories select category;
        }

        public Category getFrogById(int id)
        {
            return (from category in shopContext.Categories where category.Id == id select category).FirstOrDefault();
        }
    }
}