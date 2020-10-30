using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Abstract
{
    public interface IGeneralDoc
    {
        int Id { get; set; }
        int FkParcela { get; set; }
        string Path { get; set; }
        string Dup { get; set; }
        string GeoJson { get; set; }
    }
}