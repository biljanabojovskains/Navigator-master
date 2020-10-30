using Navigator.Models.Abstract;
using Navigator.Models.Concrete;
using NLog;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Navigator.Dal.Concrete
{
    public class TreeShrubDa
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public List<ITreeShrubSeason> GetListSeasonTree()
        //{
        //    DataTable dt;
        //    try
        //    {
        //        var cmd = Db.CreateCommand();
        //        if (cmd.Connection.State != ConnectionState.Open)
        //        {
        //            cmd.Connection.Open();
        //        }
        //        cmd.CommandText =
        //            "select distinct tree_shrub_season_name from tree_shrub_season order by tree_shrub_season_name;";
        //        dt = Db.ExecuteSelectCommand(cmd);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex);
        //        throw new Exception(ex.Message);
        //    }
        //    List<ITreeShrubSeason> list = (from DataRow dr in dt.Rows select CreateTreeShrubSeasonObject(dr)).ToList();
        //    return list;
        //}

        //public List<ITreeShrubTopology> GetTreesByName(int treeseasonid)
        //{
        //    DataTable dt;

        //    try
        //    {
        //        var cmd = Db.CreateCommand();
        //        if (cmd.Connection.State != ConnectionState.Open)
        //        {
        //            cmd.Connection.Open();
        //        }
        //        cmd.CommandText = "select distinct tree_shrub_topology_name from tree_shrub_topology where fk_tree_shrub_season =: seasontree;";
        //        Db.CreateParameterFunc(cmd, "@fk_tree_shrub_season", treeseasonid, NpgsqlDbType.Integer);
        //        dt = Db.ExecuteSelectCommand(cmd);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex);
        //        throw new Exception(ex.Message);
        //    }

        //    List<ITreeShrubTopology> list = (from DataRow dr in dt.Rows select CreateTreeShrubTopologyObject(dr)).ToList();

        //    return list;
        //}

        public List<ITreeShrubType> GetDrvoGrmushka(string search = null)
        {
          
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    @"select * from tree_shrub_type
                    where (lower(tree_shrub_type_name) like lower('%' || :search || '%') OR :search is null);";
                Db.CreateParameterFunc(cmd, "@search", search, NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<ITreeShrubType> list = (from DataRow dr in dt.Rows select CreateTreeShrubTypeObject(dr)).ToList();
            return list;
        }

        public List<IZelenilo> GetZelenilo(string coordinates)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select *, ST_AsGeoJSON(geom) as geojson from poligon  where ST_Contains(geom, ST_SetSRID(ST_MakePoint(" + coordinates + "),6316))";
                //"select *, ST_AsGeoJSON(geom) as geojson from legalizacija_za_gradba g where ST_Contains(g.geom, (select geom as geo from legalizacija_za_gradba where ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),geom))) ";


                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IZelenilo> list = (from DataRow dr in dt.Rows select CreateZeleniloSeasonObject(dr)).ToList();
            return list;
        }

             public List<ITreeShrubInventory> GetTreeShrub(string coordinates)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = @"select *, ST_AsGeoJSON(tsi.geom) as geojson 
                                from tree_shrub_inventory  tsi
                                LEFT JOIN tree_shrub_topology 
                                ON tsi.fk_tree_shrub_topology=tree_shrub_topology.tree_shrub_topology_id
                                LEFT JOIN tree_shrub_type
                                ON tree_shrub_topology.fk_tree_shrub_type=tree_shrub_type.tree_shrub_type_id
                                LEFT JOIN tree_shrub_season
                                ON tree_shrub_topology.fk_tree_shrub_season=tree_shrub_season.tree_shrub_season_id
                                LEFT JOIN condition
                                ON tsi.fk_condition=condition.condition_id
                                LEFT JOIN poligon
                                ON tsi.fk_polygon=poligon.gid
                                WHERE   (ST_Intersects(tsi.geom, ST_Buffer(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),10))) AND (fk_tree_shrub_type =1 OR fk_tree_shrub_type =2)";
                

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<ITreeShrubInventory> list = (from DataRow dr in dt.Rows select CreateTreeShrubInventoryObject(dr)).ToList();
            return list;
        }

         

        public List<ITreeShrubTopology> GetTreeShrubName(int? type = null, int?id=null, string search = null)
        {
            //NOT CLIENTS
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    @"SELECT * FROM tree_shrub_topology
                       WHERE
                       (fk_tree_shrub_type=:t or :t is null) AND
                       (fk_tree_shrub_season=:id or :id is null) AND
                       (lower(tree_shrub_topology_name) like lower('%' || :search || '%') OR :search is null);";
                Db.CreateParameterFunc(cmd, "@t", type, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@search", search, NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<ITreeShrubTopology> list = (from DataRow dr in dt.Rows select CreateTreeShrubTopologyObject(dr)).ToList();
            return list;
        }

        public List<ITreeShrubSeason> GetSeason(int? type = null, string search = null)
        {
            //NOT CLIENTS
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                //(fk_stakeholder =:sid OR :sid is null) AND
                cmd.CommandText =
                    @"SELECT * FROM tree_shrub_season
                       WHERE
                       (lower(tree_shrub_season_name) like lower('%' || :search || '%') OR :search is null);";
                //Db.CreateParameterFunc(cmd, "@sid", type, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@search", search, NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<ITreeShrubSeason> list = (from DataRow dr in dt.Rows select CreateTreeShrubSeasonObject(dr)).ToList();
            return list;
        }

        public List<ITreeShrubInventory> SearchTreeShrub(int idT, int idS, string name)
        {


            DataTable dt;

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    @"select *, ST_AsGeoJSON(tsi.geom) as geojson 
                                from tree_shrub_inventory  tsi
                                LEFT JOIN tree_shrub_topology 
                                ON tsi.fk_tree_shrub_topology=tree_shrub_topology.tree_shrub_topology_id
                                LEFT JOIN tree_shrub_type
                                ON tree_shrub_topology.fk_tree_shrub_type=tree_shrub_type.tree_shrub_type_id
                                LEFT JOIN tree_shrub_season
                                ON tree_shrub_topology.fk_tree_shrub_season=tree_shrub_season.tree_shrub_season_id
                                LEFT JOIN condition
                                ON tsi.fk_condition=condition.condition_id
                                LEFT JOIN poligon
                                ON tsi.fk_polygon=poligon.gid
                                WHERE   ";

                if (idT == 0)
                    cmd.CommandText += "(fk_tree_shrub_type !=0) AND";
                else
                    cmd.CommandText += "(fk_tree_shrub_type =:idT) AND";

                if (idS == 0)
                    cmd.CommandText += "(fk_tree_shrub_season !=0) AND";
                else
                    cmd.CommandText += "(fk_tree_shrub_season =:idS) AND";

                if (name == "")
                    cmd.CommandText += " (tree_shrub_topology_name != '');";
                else
                    cmd.CommandText += " (tree_shrub_topology_name =:name );";

                Db.CreateParameterFunc(cmd, "@idT", idT, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@idS", idS, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@name", name, NpgsqlDbType.Text);

                
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                   throw new Exception(ex.Message);
            }

            //ITreeShrubInventory list = CreateTreeShrubInventoryObject(dt.Rows[0]);
            List<ITreeShrubInventory> list = (from DataRow dr in dt.Rows select CreateTreeShrubInventoryObject(dr)).ToList();

            return list;
        }



        public ITreeShrubInventory GetTreeShrubCount(int? idT = null)
        {
            DataTable dt;

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    @"select COUNT(*) AS brojDrvaGrmushki,
					sum(case when fk_tree_shrub_season = 1 then 1 else 0 end) AS zimzeleni,
					sum(case when fk_tree_shrub_season = 2 then 1 else 0 end) AS listopadni,
					sum(case when fk_condition = 1 then 1 else 0 end) AS zdravi,
					sum(case when fk_condition = 2 then 1 else 0 end) AS bolni
				from tree_shrub_inventory  tsi
                                JOIN tree_shrub_topology 
                                ON tsi.fk_tree_shrub_topology=tree_shrub_topology.tree_shrub_topology_id
                                JOIN tree_shrub_type
                                ON tree_shrub_topology.fk_tree_shrub_type=tree_shrub_type.tree_shrub_type_id
                                JOIN tree_shrub_season
                                ON tree_shrub_topology.fk_tree_shrub_season=tree_shrub_season.tree_shrub_season_id
                                JOIN condition
                                ON tsi.fk_condition=condition.condition_id
                                JOIN poligon
                                ON tsi.fk_polygon=poligon.gid
                                WHERE   (fk_tree_shrub_type =:idT OR :idT is null);";

                Db.CreateParameterFunc(cmd, "@idT", idT, NpgsqlDbType.Integer);
               
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            ITreeShrubInventory list = CreateTreeShrubInventoryReportObject(dt.Rows[0]);

            return list;
        }


        public List<IZelenilo> GetStreets(string search = null)
        {

            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    @"select * from poligon
                    where (lower(name) like lower('%' || :search || '%') OR :search is null);";
                Db.CreateParameterFunc(cmd, "@search", search, NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IZelenilo> list = (from DataRow dr in dt.Rows select CreateZeleniloSeasonObject(dr)).ToList();
            return list;
        }

        public List<IZelenilo> GetUlica(string name)
        {
            DataTable dt;

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    @"SELECT *, ST_AsGeoJSON(geom) as geojson FROM poligon
                    WHERE (name =:name);";



                Db.CreateParameterFunc(cmd, "@name", name, NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }

            List<IZelenilo> list = (from DataRow dr in dt.Rows select CreateZeleniloSeasonObject(dr)).ToList();

            return list;
        }



        private static ITreeShrubInventory CreateTreeShrubInventoryObject(DataRow dr)
        {
            var treeShrubInventory = new TreeShrubInventory
            {
                Id = int.Parse(dr["tree_shrub_inventory_id"].ToString()),
                TreeShrubTypeName = dr["tree_shrub_type_name"].ToString(),
                SeasonName = dr["tree_shrub_season_name"].ToString(),
                Fk_Topology = (dr["fk_tree_shrub_topology"] == null || dr["fk_tree_shrub_topology"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_tree_shrub_topology"].ToString()),
                TopologyName = dr["tree_shrub_topology_name"].ToString(),
                LatinTopologyName = dr["tree_shrub_topology_latin_name"].ToString(),
                Height = (dr["height"] == null || dr["height"].ToString() == "")
                    ? (double?)null
                    : double.Parse(dr["height"].ToString()),
                CanopyWidth = (dr["canopy_width"] == null || dr["canopy_width"].ToString() == "")
                    ? (double?)null
                    : double.Parse(dr["canopy_width"].ToString()),
                Age = (dr["age"] == null || dr["age"].ToString() == "")
                    ? (double?)null
                    : double.Parse(dr["age"].ToString()),
                Fk_Condition = (dr["fk_condition"] == null || dr["fk_condition"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_condition"].ToString()),
                ConditionName = dr["condition_name"].ToString(),
                Intervention = dr["intervention"].ToString(),
                Fk_Polygon = (dr["fk_polygon"] == null || dr["fk_polygon"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_polygon"].ToString()),
                PolygonName = dr["name"].ToString(),
                DateCreated = (dr["date_created"] == null || dr["date_created"].ToString() == "")
                    ? (DateTime?)null
                    : DateTime.Parse(dr["date_created"].ToString()),
                IdNumber = (dr["id_number"] == null || dr["id_number"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["id_number"].ToString()),
                Note = dr["note"].ToString(),
                CreatedBy = (dr["created_by"] == null || dr["created_by"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["created_by"].ToString()),
                GeoJson = dr["geojson"].ToString(),

            };
            return treeShrubInventory;
        }

        private static ITreeShrubInventory CreateTreeShrubInventoryReportObject(DataRow dr)
        {
            var treeShrubInventory = new TreeShrubInventory
            {
                
                CountTreeShrub = int.Parse(dr["brojDrvaGrmushki"].ToString()),
                CountZimzeleni = int.Parse(dr["zimzeleni"].ToString()),
                CountListopadni = int.Parse(dr["listopadni"].ToString()),
                CountZdravi = int.Parse(dr["zdravi"].ToString()),
                CountBolni = int.Parse(dr["bolni"].ToString())

            };
            return treeShrubInventory;
        }
        private static ITreeShrubSeason CreateTreeShrubSeasonObject(DataRow dr)
        {
            var treeShrubSeason = new TreeShrubSeason
            {
                Id = dr["tree_shrub_season_id"].ToString(),
                Name = dr["tree_shrub_season_name"].ToString()
            };
            return treeShrubSeason;
        }

        private static ITreeShrubTopology CreateTreeShrubTopologyObject(DataRow dr)
        {
            var treeShrubTopology = new TreeShrubTopology
            {
                Id = int.Parse(dr["tree_shrub_topology_id"].ToString()),
                Name = dr["tree_shrub_topology_name"].ToString(),
                LatinName = dr["tree_shrub_topology_latin_name"].ToString(),
                Fk_Type = (dr["fk_tree_shrub_type"] == null || dr["fk_tree_shrub_type"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_tree_shrub_type"].ToString()),
                Fk_Season = (dr["fk_tree_shrub_season"] == null || dr["fk_tree_shrub_season"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_tree_shrub_season"].ToString())
            };
            return treeShrubTopology;
        }
        private static ITreeShrubType CreateTreeShrubTypeObject(DataRow dr)
        {
            var treeShrubType = new TreeShrubType
            {
                Id = int.Parse(dr["tree_shrub_type_id"].ToString()),
                Type = dr["tree_shrub_type_name"].ToString()
            };
            return treeShrubType;
        }


        private static IZelenilo CreateZeleniloSeasonObject(DataRow dr)
        {
            var zelenilo = new Zelenilo
            {
                Id = int.Parse(dr["gid"].ToString()),
                Name = dr["name"].ToString(),
                Povrsina = (dr["povrsina"] == null || dr["povrsina"].ToString() == "")
                    ? (double?)null
                    : double.Parse(dr["povrsina"].ToString()),
                KP = dr["kp"].ToString(),
                KO = dr["ko"].ToString(),
                GeoJson = dr["geom"].ToString()

            };
            try
            {
                zelenilo.GeoJsonUlica = dr["geojson"].ToString();
            }
            catch {
                // ignored
            }

            return zelenilo;
        }


    }
}