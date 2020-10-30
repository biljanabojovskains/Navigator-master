using System;
namespace Navigator.Models.Abstract

{
    public interface IGradParceli
    {
        int Id { get; set; }
        string Broj { get; set; }
        string KlasaNamena { get; set; }
        string KompKlasaNamena { get; set; }
        /// <summary>
        /// -1 Постојна состојба
        /// -2 со Архитектонско-урбанистички проект
        /// -3 според АУП
        /// >0 вистинска површина
        /// </summary>
        double? Povrshina { get; set; }
        /// <summary>
        /// -1 Постојна состојба
        /// -2 со Архитектонско-урбанистички проект
        /// -3 според АУП
        /// >0 вистинска површина
        /// </summary>
        double? PovrshinaGradenje { get; set; }
        /// <summary>
        /// -1 Постојна состојба
        /// -2 со Архитектонско-урбанистички проект
        /// -3 според АУП
        /// >0 вистинска површина
        /// </summary>
        double? BrutoPovrshina { get; set; }
        string MaxVisina { get; set; }
        double? Extrude { get; set; }
        string Katnost { get; set; }
        string ParkingMesta { get; set; }
        double? ProcentIzgradenost { get; set; }
        double? ProcentIzgradenostPresmetana { get; }
        double? KoeficientIskoristenost { get; set; }
        double? PovrshinaPresmetana { get; set; }
        string GeoJson { get; set; }
        int? OpstiUsloviId { get; set; }
        int? PosebniUsloviId { get; set; }
        double? Presek { get; set; }
        int OpfatId { get; set; }
        string ProcentIzgradenostOpisno { get; }
        string KoeficientIskoristenostOpisno { get; }
        string PovrshinaOpisno { get; }
        string PovrshinaGradenjeOpisno { get; }
        string BrutoPovrshinaOpisno { get; }
        string Investitor { get; set; }
        int PovrshinaZaokruzena { get; }
        int BrutoPovrshinaZaokruzena { get; }
        int PovrshinaGradenjeZaokruzena { get; }
        int? NumerickiPokazateliId { get; set; }
        string  BrTehIspravka { get; set; }
        DateTime? DatumTehIspravka { get; set; }
        int? TehnickiIspravkiId { get; set; }
    }
}
