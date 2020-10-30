using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Abstract
{
    public interface IUlicaInfo
    {
        int Id { get; set; }
        string Ime_ulica { get; set; }
        string GeoJson { get; set; }
      
    }
}