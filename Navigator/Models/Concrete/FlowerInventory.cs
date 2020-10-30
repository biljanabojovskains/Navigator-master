using Navigator.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Concrete
{
    public class FlowerInventory : IFlowerInventory
    {
        public int Id { get; set; }
        public int? Fk_FlowerTopology { get; set; }
        public string FlowerTypeName { get; set; }
        public string FlowerSeason { get; set; }
        public string FlowerName { get; set; }
        public string FlowerLatinName { get; set; }
        public int? Fk_Type { get; set; }
        public int? Fk_Condition { get; set; }
        public string ConditionName { get; set; }
        public string Intervention { get; set; }
        public int? Fk_Polygon { get; set; }
        public string PolygonName { get; set; }
        public string Note { get; set; }
        public int? IdNumber { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string GeoJson { get; set; }
        public double? Surface { get; set; }
    }
}