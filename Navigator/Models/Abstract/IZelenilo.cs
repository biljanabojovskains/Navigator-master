using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Abstract
{
    public interface IZelenilo
    {
        int Id { get; set; }
        string Name { get; set; }
        double? Povrsina { get; set; }
        string KP { get; set; }
        string KO { get; set; }
        string GeoJson { get; set; }
        string GeoJsonUlica { get; set; }
    }
}