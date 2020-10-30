using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigator.Models.Abstract
{
    public interface IFlowerInventory
    {
        int Id { get; set; }
        int? Fk_FlowerTopology { get; set; }
        string FlowerTypeName { get; set; }
        string FlowerSeason { get; set; }
        string FlowerName { get; set; }
        string FlowerLatinName { get; set; }
        int? Fk_Type { get; set; }
        int? Fk_Condition { get; set; }
        string ConditionName { get; set; }
        string Intervention { get; set; }
        int? Fk_Polygon { get; set; }
        string PolygonName { get; set; }
        string Note { get; set; }
        int? IdNumber { get; set; }
        int? CreatedBy { get; set; }
        DateTime? DateCreated { get; set; }
        string GeoJson { get; set; }
        double? Surface { get; set; }
    }
}
