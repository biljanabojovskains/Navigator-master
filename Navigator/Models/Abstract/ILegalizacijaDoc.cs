using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Abstract
{
    public class ILegalizacijaDoc
    {
        int Id { get; set; }
        string Path { get; set; }
        DateTime Datum { get; set; }
        string Filename { get; set; }

    }
}