using Navigator.Models.Abstract;
using Navigator.Models.Concrete;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Navigator.Dal.Concrete
{
    public class FlowerDa
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public List<IFlowerInventory> GetCvekjinja(string coordinates)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = @"    select *, ST_AsGeoJSON(fi.geom) as geojson 
                                from flower_inventory  fi
                                LEFT JOIN flower_topology 
                                ON fi.fk_flower_topology=flower_topology.flower_topology_id
                                LEFT JOIN flower_type
                                ON fi.fk_flower_type=flower_type.flower_type_id
                                LEFT JOIN flower_season
                                ON flower_topology.fk_flower_season=flower_season.flower_season_id
                                LEFT JOIN condition
                                ON fi.fk_condition=condition.condition_id
                                LEFT JOIN poligon
                                ON fi.fk_polygon=poligon.gid
                                WHERE  (ST_Intersects(fi.geom, ST_Buffer(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),10))) AND (fk_flower_type =1 OR fk_flower_type =2 OR fk_flower_type=3);";


                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IFlowerInventory> list = (from DataRow dr in dt.Rows select CreateFlowerInventoryObject(dr)).ToList();
            return list;
        }

        private static IFlowerInventory CreateFlowerInventoryObject(DataRow dr)
        {
         
            var flowerInventory = new FlowerInventory
            {
                Id = int.Parse(dr["flower_inventory_id"].ToString()),
                Fk_FlowerTopology = (dr["fk_flower_topology"] == null || dr["fk_flower_topology"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_flower_topology"].ToString()),
                FlowerTypeName = dr["flower_type_name"].ToString(),
                FlowerSeason = dr["flower_season_name"].ToString(),
                FlowerName = dr["flower_topology_name"].ToString(),
                FlowerLatinName = dr["flower_topology_latin_name"].ToString(),
                Fk_Type = (dr["fk_flower_type"] == null || dr["fk_flower_type"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_flower_type"].ToString()),
                Fk_Condition = (dr["fk_condition"] == null || dr["fk_condition"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_condition"].ToString()),
                ConditionName = dr["condition_name"].ToString(),
                Intervention = dr["intervention"].ToString(),
                Fk_Polygon = (dr["fk_polygon"] == null || dr["fk_polygon"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_polygon"].ToString()),
                PolygonName = dr["name"].ToString(),
                Note = dr["note"].ToString(),
                IdNumber = (dr["id_number"] == null || dr["id_number"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["id_number"].ToString()),
                CreatedBy = (dr["created_by"] == null || dr["created_by"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["created_by"].ToString()),
                DateCreated = (dr["date_created"] == null || dr["date_created"].ToString() == "")
                    ? (DateTime?)null
                    : DateTime.Parse(dr["date_created"].ToString()),
                GeoJson = dr["geojson"].ToString(),
                Surface = (dr["surface"] == null || dr["surface"].ToString() == "")
                    ? (double?)null
                    : double.Parse(dr["surface"].ToString())
            };
            return flowerInventory;
        }

        private static IFlowerSeason CreateFlowerSeasonObject(DataRow dr)
        {
            var flowerSeason = new FlowerSeason
            {
                Id = int.Parse(dr["flower_season_id"].ToString()),
                Name = dr["flower_season_name"].ToString()
            };
            return flowerSeason;
        }
        private static IFlowerTopology CreateFlowerTopologyObject(DataRow dr)
        {
            var flowerTopology = new FlowerTopology
            {
                Id = int.Parse(dr["flower_topology_id"].ToString()),
                Name = dr["flower_topology_name"].ToString(),
                LatinName = dr["flower_topology_latin_name"].ToString(),
                Fk_Season = (dr["fk_flower_season"] == null || dr["fk_flower_season"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_flower_season"].ToString())
                    
            };
            return flowerTopology;
        }
        private static IFlowerType CreateFlowerTypeObject(DataRow dr)
        {
            var flowerType = new FlowerType
            {
                Id = int.Parse(dr["flower_type_id"].ToString()),
                Name = dr["flower_type_name"].ToString(),
                LatinName = dr["flower_type_latin_name"].ToString(),
                Fk_Topology = (dr["fk_flower_topology"] == null || dr["fk_flower_topology"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_flower_topology"].ToString())
                    
            };
            return flowerType;
        }




    }
}