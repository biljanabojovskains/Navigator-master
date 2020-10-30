using Navigator.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Concrete
{
    public class GreenSurfaceInventory : IGreenSurfaceInventory
    {
       public int Id { get; set; }
        //public int Fk_Topology { get; set; }
       public int? Fk_Condition { get; set; }
       public string ConditionName { get; set; }
       public DateTime? DateCreated { get; set; }
       public string Note { get; set; }
       public double? Surface { get; set; }
       public int? CreatedBy { get; set; }
       public int? Fk_Polygon { get; set; }
       public string PolygonName { get; set; }
       public double? Paths { get; set; }
       public int? Fk_Season { get; set; }
       public string SeasonName { get; set; }
       public string Intervention { get; set; }
       public string GeoJson { get; set; }
      
      
    }
}