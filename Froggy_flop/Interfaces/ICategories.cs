using Froggy_flop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Froggy_flop.Interfaces
{
    public interface ICategories
    {
        IEnumerable<Category> All();
        Category GetCategoryById(int id);
    }
}