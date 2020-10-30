using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicNavigator.Models.Abstract
{
    public interface IBiznisInfo
    {
        int Id { get; set; }
        string Ime { get; set; }
        string KontaktLice { get; set; }
        string Telefon { get; set; }
        string Email { get; set; }
        string Adresa { get; set; }
        string Web { get; set; }
        string GeoJson { get; set; }
    }
}