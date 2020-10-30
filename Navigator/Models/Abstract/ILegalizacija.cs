using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Abstract
{
    public class ILegalizacija
    {
        int Id { get; set; }
        int FkParcela { get; set; }
        string KatastarskaOpstina { get; set; }
        string BrKatastarskaParcela { get; set; }
        string BrPredmet { get; set; }
        string NamenaNaObjekt { get; set; }
        int? BrojObjekt { get; set; }
        int TipLegalizacija { get; set; }
        int FkLegalizacijaDoc { get; set; }
        string GeoJson { get; set; }
        bool Active { get; set; }
        string GeoJsonParceli { get; set; }
        int IdDoc { get; set; }
        string Path { get; set; }
        string Filename { get; set; }
        string Legalizirani { get; set; }
        int Count { get; set; }
        
    }
}