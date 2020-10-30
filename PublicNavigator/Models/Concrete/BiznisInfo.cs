using System;
using PublicNavigator.Models.Abstract;
using System.Collections.Generic;

namespace PublicNavigator.Models.Concrete
{
    public class BiznisInfo : IBiznisInfo
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string KontaktLice { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string Adresa { get; set; }
        public string Web { get; set; }
        public string GeoJson { get; set; }
    }
}