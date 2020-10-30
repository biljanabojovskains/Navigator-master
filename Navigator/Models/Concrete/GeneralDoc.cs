using System;
using Navigator.Models.Abstract;
using System.Collections.Generic;

namespace Navigator.Models.Concrete
{
    public class GeneralDoc : IGeneralDoc
    {
        public int Id { get; set; }
        public int FkParcela { get; set; }
        public string Path { get; set; }
        public string Dup { get; set; }       
        public string GeoJson { get; set; }
    }
}