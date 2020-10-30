using Navigator.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Concrete
{
    public class Zelenilo: IZelenilo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double? Povrsina { get; set; }
        public string KP { get; set; }
        public string KO { get; set; }
        public string GeoJson { get; set; }
        public string GeoJsonUlica { get; set; }
    }
}