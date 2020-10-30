using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    [Serializable]
    public class UlicaInfo: IUlicaInfo
    {
        public int Id { get; set; }
        public string Ime_ulica { get; set; }
        public string GeoJson { get; set; }


    }
}