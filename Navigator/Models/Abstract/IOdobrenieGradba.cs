using System;

namespace Navigator.Models.Abstract
{
    public interface IOdobrenieGradba
    {
        int Id { get; set; }
        int FkParcela { get; set; }
        string BrPredmet { get; set; }
        string TipBaranje { get; set; }
        string PodTipBaranje { get; set; }
        string Sluzbenik { get; set; }
        DateTime DatumBaranja { get; set; }
        DateTime DatumIzdavanja { get; set; }
        DateTime DatumPravosilno { get; set; }
        string Investitor { get; set; }
        string BrKP { get; set; }
        string KO { get; set; }
        string adresa { get; set; }
        string ParkingMestaPacela { get; set; }
        string ParkingMestaGaraza { get; set; }
        string KatnaGaraza { get; set; }
        double IznosKomunalii { get; set; }
        string Zabeleski { get; set; }
        string Path { get; set; }
        string Dup { get; set; }
        string OdlukaDup { get; set; }
        DateTime DonesuvanjeOdlukaDup { get; set; }
        string Namena { get; set; }
        string BrNamena { get; set; }
        string GeoJson { get; set; }
    }
}