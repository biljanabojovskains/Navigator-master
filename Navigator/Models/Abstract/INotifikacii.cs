using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Abstract
{
    public interface INotifikacii
    {
        int Id { get; set; }
        string Tema { get; set; }
        string Podtema { get; set; }
        DateTime DatumOd { get; set; }
        DateTime DatumDo { get; set; }
        string GeoJson { get; set; }
    }
}