using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
     [Serializable]
    public class Segment: ISegment
    {
        public int Id { get; set; }
        public string Ime_ulica { get; set; }
        public string Tip_Ulica { get; set; }
        public int SegmentBr { get; set; }
        public double Shirina { get; set; }
        public bool Trotoari { get; set; }
        public bool Velosipedska_pateka { get; set; }
        public bool Zelenilo { get; set; }
        public bool Atmosferska_planirana { get; set; }
        public bool Atmosferska_postojna { get; set; }
        public bool Vodovodna_planirana { get; set; }
        public bool Vodovodna_postojna { get; set; }
        public bool Gasovodna_planirana { get; set; }
        public bool Gasovodna_postojna { get; set; }
        public bool Telekomunikaciska_planirana { get; set; }
        public bool Telekomunikaciska_postojna { get; set; }
        public bool Elektrika_planirana { get; set; }
        public bool Elektrika_postojna { get; set; }
        public bool Fekalna_planirana { get; set; }
        public bool Fekalna_postojna { get; set; }
        public bool Toplifikacija_planirana { get; set; }
        public bool Toplifikacija_postojna { get; set; }
        public string GeoJson { get; set; }
        public bool Parking { get; set; }
        public bool Pesacka_pateka { get; set; }



    }
}