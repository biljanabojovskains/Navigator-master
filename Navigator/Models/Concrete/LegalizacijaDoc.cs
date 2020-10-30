using System;
using Navigator.Models.Abstract;
using System.Collections.Generic;


namespace Navigator.Models.Concrete
{
    public class LegalizacijaDoc : ILegalizacijaDoc
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public DateTime Datum { get; set; }
        public string Filename { get; set; }
    }
}