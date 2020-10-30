using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Abstract
{
    public interface ISegment
    {
        int Id { get; set; }
        string Ime_ulica { get; set; }
        string Tip_Ulica { get; set; }
        int SegmentBr { get; set; }
        double Shirina { get; set; }
        bool Trotoari { get; set; }
        bool Velosipedska_pateka { get; set; }
        bool Zelenilo { get; set; }
        bool Atmosferska_planirana { get; set; }
        bool Atmosferska_postojna { get; set; }
        bool Vodovodna_planirana { get; set; }
        bool Vodovodna_postojna { get; set; }
        bool Gasovodna_planirana { get; set; }
        bool Gasovodna_postojna { get; set; }
        bool Telekomunikaciska_planirana { get; set; }
        bool Telekomunikaciska_postojna { get; set; }
        bool Elektrika_planirana { get; set; }
        bool Elektrika_postojna { get; set; }
        bool Fekalna_planirana { get; set; }
        bool Fekalna_postojna { get; set; }
        bool Toplifikacija_planirana { get; set; }
        bool Toplifikacija_postojna { get; set; }
        string GeoJson { get; set; }

        bool Parking { get; set; }
        bool Pesacka_pateka { get; set; }
       
    }
}