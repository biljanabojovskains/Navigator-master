using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    [Serializable]
    public class UlicaSegment: IUlicaSegment
    {
        public int Id { get; set; }
        public int Fk_ulica_id { get; set; }
        public int Fk_segment_ulica_id { get; set; }

    }
}