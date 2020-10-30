using Navigator.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Concrete
{
    public class TreeShrubInventory : ITreeShrubInventory
    {
       public int Id { get; set; }
       public string TreeShrubTypeName { get; set; }
       public string SeasonName { get; set; }
       public int? Fk_Topology { get; set; }
       public string TopologyName { get; set; }
       public string LatinTopologyName { get; set; }
       public double? Height { get; set; }
       public double? CanopyWidth { get; set; }
       public double? Age { get; set; }
       public int? Fk_Condition { get; set; }
       public string ConditionName { get; set; }
       public string Intervention { get; set; }
       public int? Fk_Polygon { get; set; }
       public string PolygonName { get; set; }
       public DateTime? DateCreated { get; set; }
       public int? IdNumber { get; set; }
       public string Note { get; set; }
       public int? CreatedBy { get; set; }
       public string GeoJson { get; set; }
       public int CountTreeShrub { get; set; }
       public int CountZimzeleni { get; set; }
       public int CountListopadni { get; set; }
       public int CountZdravi { get; set; }
       public int CountBolni { get; set; }
       
    }
}