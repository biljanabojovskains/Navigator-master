using System;

namespace PublicNavigator.Models.Abstract
{
    public interface IOpfat
    {
        int Id { get; set; }
        string Ime { get; set; }
        string PlanskiPeriod { get; set; }
        string BrOdluka { get; set; }
        DateTime? DatumNaDonesuvanje { get; set; }
        string TehnickiBroj { get; set; }
        string ZakonskaRegulativa { get; set; }
        string Izrabotuva { get; set; }
        double? Povrshina { get; set; }
        double? PovrshinaPresmetana { get; set; }
        string GeoJson { get; set; }
    }
}
