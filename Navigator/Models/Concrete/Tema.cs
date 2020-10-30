using Navigator.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Concrete
{
    public class Tema : ITema
    {
        public int Id { get; set; }
        public string ImeTema { get; set; }
    }
}