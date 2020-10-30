using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigator.Models.Abstract
{
    public interface IGreenSurfaceInventory
    {
        int Id { get; set; }
        //int Fk_Topology { get; set; }
        int? Fk_Condition { get; set; }
        string ConditionName { get; set; }
        DateTime? DateCreated { get; set; }
        string Note { get; set; }
        double? Surface { get; set; }
        int? CreatedBy { get; set; }
        int? Fk_Polygon { get; set; }
        string PolygonName { get; set; }
        double? Paths { get; set; }
        int? Fk_Season { get; set; }
        string SeasonName { get; set; }
        string Intervention { get; set; }
        string GeoJson { get; set; }
        
        
        
    }
}
