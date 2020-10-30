using System;
using Navigator.Models.Abstract;
using System.Collections.Generic;

namespace Navigator.Models.Concrete
{
    public class OdobrenieGradba : IOdobrenieGradba
    {
        public int Id { get; set; }
        public int FkParcela { get; set; }
        public string BrPredmet { get; set; }
        public string TipBaranje { get; set; }
        public string PodTipBaranje { get; set; }
        public string Sluzbenik { get; set; }
        public DateTime DatumBaranja { get; set; }
        public DateTime DatumIzdavanja { get; set; }
        public DateTime DatumPravosilno { get; set; }
        public string Investitor { get; set; }
        public string BrKP { get; set; }
        public string KO { get; set; }
        public string adresa { get; set; }
        public string ParkingMestaPacela { get; set; }
        public string ParkingMestaGaraza { get; set; }
        public string KatnaGaraza { get; set; }
        public double IznosKomunalii { get; set; }
        public string Zabeleski { get; set; }
        public string Path { get; set; }
        public string Dup { get; set; }
        public string OdlukaDup { get; set; }
        public DateTime DonesuvanjeOdlukaDup { get; set; }
        public string Namena { get; set; }
        public string BrNamena { get; set; }
        public string GeoJson { get; set; }
    }
}