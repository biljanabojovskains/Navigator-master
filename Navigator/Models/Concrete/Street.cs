using Navigator.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Concrete
{
    public class Street : IStreet
    {
        public string id { get; set; }
        public string text { get; set; }
    }
}