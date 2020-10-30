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
    public class GreenSurfaceDa
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public List<IGreenSurfaceInventory> GetZeleniPovrsini(string coordinates)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = @"  select *, ST_AsGeoJSON(gfi.geom) as geojson 
                                from green_surface_inventory  gfi
                                LEFT JOIN green_surface_season
                                ON gfi.fk_green_surface_season=green_surface_season.green_surface_season_id
                                LEFT JOIN condition
                                ON gfi.fk_condition=condition.condition_id
                                LEFT JOIN poligon
                                ON gfi.fk_polygon=poligon.gid
                                WHERE  (ST_Intersects(gfi.geom, ST_Buffer(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),10))) ;";


                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IGreenSurfaceInventory> list = (from DataRow dr in dt.Rows select CreateGreenSurfaceInventoryObject(dr)).ToList();
            return list;
        }


        private static IGreenSurfaceInventory CreateGreenSurfaceInventoryObject(DataRow dr)
        {
            var greebnSurfaceInventory = new GreenSurfaceInventory
            {
                Id = int.Parse(dr["green_surface_inventory_id"].ToString()),
                //Fk_Topology = int.Parse(dr["fk_green_surface_topology"].ToString()),
                Fk_Condition = (dr["fk_condition"] == null || dr["fk_condition"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_condition"].ToString()),
                ConditionName = dr["condition_name"].ToString(),
                DateCreated = (dr["date_created"] == null || dr["date_created"].ToString() == "")
                    ? (DateTime?)null
                    : DateTime.Parse(dr["date_created"].ToString()),
                Note = dr["note"].ToString(),
                Surface = (dr["surface"] == null || dr["surface"].ToString() == "")
                    ? (double?)null
                    : double.Parse(dr["surface"].ToString()),
                CreatedBy = (dr["created_by"] == null || dr["created_by"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["created_by"].ToString()),
                Fk_Polygon = (dr["fk_polygon"] == null || dr["fk_polygon"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_polygon"].ToString()),
                PolygonName = dr["name"].ToString(),
                Paths = (dr["paths"] == null || dr["paths"].ToString() == "")
                    ? (double?)null
                    : double.Parse(dr["paths"].ToString()),
                Fk_Season = (dr["fk_green_surface_season"] == null || dr["fk_green_surface_season"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_green_surface_season"].ToString()),
                SeasonName = dr["green_surface_season_name"].ToString(),
                Intervention = dr["intervention"].ToString(),
                GeoJson = dr["geojson"].ToString()
            };
            return greebnSurfaceInventory;
        }

        private static IGreenSurfaceSeason CreateGreenSurfaceSeasonObject(DataRow dr)
        {
            var greenSurfaceSeason = new GreenSurfaceSeason
            {
                Id = int.Parse(dr["green_surface_season_id"].ToString()),
                Name = dr["green_surface_season_name"].ToString()
            };
            return greenSurfaceSeason;
        }

        private static IGreenSurfaceTopology CreateGreenSurfaceTopologyObject(DataRow dr)
        {
            var greenSurfaceTopology = new GreenSurfaceTopology
            {
                Id = int.Parse(dr["green_surface_topology_id"].ToString()),
                Name = dr["green_surface_topology_name"].ToString(),
                Fk_Season = (dr["fk_green_surface_season"] == null || dr["fk_green_surface_season"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_green_surface_season"].ToString())
            };
            return greenSurfaceTopology;
        }
    }
}