using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Abstract
{
    public interface IUlicaSegment
    {
        int Id { get; set; }
        int Fk_ulica_id { get; set; }
        int Fk_segment_ulica_id { get; set; }
    }
}