using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicNavigator.Models.Abstract
{
    public interface IPodTema 
    {
        int id { get; set; }
        string text { get; set; }
    }
}