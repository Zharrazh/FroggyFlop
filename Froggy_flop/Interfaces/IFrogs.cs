using Froggy_flop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Froggy_flop.Interfaces
{
    public interface IFrogs
    {
        IEnumerable<Frog> All();
        Frog getFrogById(int id);
    }
}