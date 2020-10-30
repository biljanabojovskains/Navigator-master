using System;
using Navigator.Models.Abstract;
using System.Collections.Generic;

namespace Navigator.Models.Concrete
{
    public class Legalizacija : ILegalizacija
    {
        public int Id { get; set; }
        public int? FkParcela { get; set; }
        public string KatastarskaOpstina { get; set; }
        public string BrKatastarskaParcela { get; set; }
        public string BrPredmet { get; set; }
        public string NamenaNaObjekt { get; set; }
        public int? BrojObjekt { get; set; }
        public string TipLegalizacija { get; set; }
        public int? FkLegalizacijaDoc { get; set; }
        public string GeoJson { get; set; }
        public bool Active { get; set; }
        public string GeoJsonParceli { get; set; }
        public int IdDoc { get; set; }
        public string Path { get; set; }
        public string Filename { get; set; }
        public string Legalizirani { get; set; }
        public int Count { get; set; }
        public DateTime Datum { get; set; }

    }
}