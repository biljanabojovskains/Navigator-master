using PublicNavigator.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicNavigator.Models.Concrete
{
    public class PodTema : IPodTema
    {
        public int id { get; set; }
        public string text { get; set; }
    }
}