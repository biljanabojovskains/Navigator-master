﻿using System;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class Opfat : IOpfat
    {
        public int Id { get; set; }

        public string Ime { get; set; }

        public string PlanskiPeriod { get; set; }

        public string TehnickiBroj { get; set; }

        public double? Povrshina { get; set; }
        
        public double? PovrshinaPresmetana { get; set; }

        public string GeoJson { get; set; }

        public string BrOdluka { get; set; }

        public DateTime? DatumNaDonesuvanje { get; set; }

        public string ZakonskaRegulativa { get; set; }

        public string Izrabotuva { get; set; }
        public int TipPlan { get; set; }
        public string SlVesnik { get; set; }
      
    }
}
