using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigator.Models.Abstract
{
    public interface ITreeShrubInventory
    {
        int Id { get; set; }
        string TreeShrubTypeName { get; set; }
        string SeasonName { get; set; }
        int? Fk_Topology { get; set; }
        string TopologyName { get; set; }
        string LatinTopologyName { get; set; }
        double? Height { get; set; }
        double? CanopyWidth { get; set; }
        double? Age { get; set; }
        int? Fk_Condition { get; set; }
        string ConditionName { get; set; }
        string Intervention { get; set; }
        int? Fk_Polygon { get; set; }
        string PolygonName { get; set; }
        DateTime? DateCreated { get; set; }
        int? IdNumber { get; set; }
        string Note { get; set; }
        int? CreatedBy { get; set; }
        string GeoJson { get; set; }
        int CountTreeShrub { get; set; }
        int CountZimzeleni { get; set; }
        int CountListopadni { get; set; }
        int CountZdravi { get; set; }
        int CountBolni { get; set; }
    }
}
