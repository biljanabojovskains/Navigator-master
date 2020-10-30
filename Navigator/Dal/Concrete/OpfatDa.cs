using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Globalization;
using Navigator.Dal.Abstract;
using Navigator.Models.Abstract;
using Navigator.Models.Concrete;
using NLog;
using NpgsqlTypes;

namespace Navigator.Dal.Concrete
{
    public class OpfatDa : IBaseDal<IOpfat>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public List<IOpfat> GenerateList(string coordinates)
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
                    "select * from mikro_opfat where active=true and valid_to='infinity' and ST_Intersects(ST_SetSRID(ST_MakePoint(" +
                    coordinates +
                    "),6316),geom);";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IOpfat> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }
        public List<IOpfat2> GetListOpfat()
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
                    "select distinct opfat_ime from mikro_opfat  where active=true and valid_to='infinity' order by opfat_ime;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IOpfat2> list = (from DataRow dr in dt.Rows select CreateOpfatObject(dr)).ToList();
            return list;
        }
        private IOpfat2 CreateOpfatObject(DataRow dr)
        {
            var opfat = new Opfat2
            {
                id = dr["opfat_ime"].ToString(),
                text = dr["opfat_ime"].ToString()
            };
            return opfat;
        }
        private IOpfat CreateObject(DataRow dr)
        {
            var item = new Opfat
            {
                Id = int.Parse(dr["mikro_opfat_id"].ToString()),
                Ime = dr["opfat_ime"].ToString(),
                TehnickiBroj = dr["tehnicki_broj"].ToString(),
                Povrshina = (dr["povrsina"] == null || dr["povrsina"].ToString() == "")
                    ? (double?) null
                    : double.Parse(dr["povrsina"].ToString()),
                PovrshinaPresmetana = (dr["povrsina_presmetana"] == null || dr["povrsina_presmetana"].ToString() == "")
                    ? (double?) null
                    : double.Parse(dr["povrsina_presmetana"].ToString()),
                PlanskiPeriod = dr["planski_period"].ToString(),
                ZakonskaRegulativa = dr["zakonska_regulativa"].ToString(),
                Izrabotuva = dr["izrabotuva"].ToString(),
                TipPlan = int.Parse(dr["fk_tip_plan"].ToString()),
                BrOdluka = dr["br_odluka"].ToString(),               
                DatumNaDonesuvanje = (dr["datum_donesuvanje"] == null || dr["datum_donesuvanje"].ToString() == "")
                    ? (DateTime?) null
                    : DateTime.Parse(dr["datum_donesuvanje"].ToString())
            };
            try
            {
                item.GeoJson = dr["geojson"].ToString();
            }
            catch
            {
                // ignored
            }
            try
            {
                item.SlVesnik = dr["sl_vesnik"].ToString();
            }
            catch
            {
                // ignored
            }
            return item;
        }


        public IVertex GetCentroidById(int id)
        {
            throw new NotImplementedException();
        }

        public List<IOpfat> Search(string keyword)
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
                    "select *, ST_AsGeoJSON(geom) as geojson from mikro_opfat where active=true and valid_to='infinity' and opfat_ime ilike :kw ;";
                Db.CreateParameterFunc(cmd, "@kw", '%' + keyword + '%', NpgsqlDbType.Text);
            
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IOpfat> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        public List<IOpfat> GetAllTekovni()
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
                    "select * from mikro_opfat where active=true and valid_to='infinity' and produkcija=true;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IOpfat> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        public List<IOpfat> GetAllNedoneseni()
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select * from mikro_opfat where produkcija=false;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IOpfat> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        public IOpfat Get(int id)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select *, ST_AsGeoJSON(geom) as geojson from mikro_opfat where mikro_opfat_id=:id;";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            return CreateObject(dt.Rows[0]);
        }

      

        public List<IOpfat> ListaOpfat(string poligon)
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
                    //"select *, ST_AsGeoJSON(geom) as geojson from mikro_opfat where active=true and valid_to='infinity' and ST_Intersects(ST_SetSRID(ST_MakePoint(" +
                    //coordinates +
                    //"),6316),geom);";

                "select * ,ST_AsGeoJSON(geom) as geojson from mikro_opfat where active=true and valid_to='infinity' and  ST_Intersects(geom, ST_GeomFromText('POLYGON(( " + poligon + "))', 6316)) order by datum_donesuvanje desc limit 1;";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IOpfat> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        public List<IOpfat> ListaOpfatAerodrom(string poligon)
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
                    //"select *, ST_AsGeoJSON(geom) as geojson from mikro_opfat where active=true and valid_to='infinity' and ST_Intersects(ST_SetSRID(ST_MakePoint(" +
                    //coordinates +
                    //"),6316),geom);";

                "select * ,ST_AsGeoJSON(geom) as geojson from mikro_opfat where active=true and valid_to='infinity' and  ST_Intersects(geom, ST_GeomFromText('POLYGON(( " + poligon + "))', 6316));";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IOpfat> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        public List<IOpfat> ListaOpfatUlicaAerodrom(string poligon, int id)
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

                //"select mikro_opfat.* , ulica_info.* from mikro_opfat,ulica_info where mikro_opfat.active=true and mikro_opfat.valid_to='infinity' and ST_Intersects(mikro_opfat.geom, ulica_info.geom) and ulica_info.ulica_id=:id order by mikro_opfat.opfat_ime;";

                "select mikro_opfat.*, ulica_info.* from mikro_opfat,ulica_info where mikro_opfat.active=true and mikro_opfat.valid_to='infinity' and ST_Intersects(mikro_opfat.geom, ulica_info.geom) and ulica_info.ulica_id=:id and ST_Intersects( mikro_opfat.geom, ST_SetSRID(ST_GeomFromText('POLYGON(( " + poligon + "))'),6316));";

                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IOpfat> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }
    

        //sredisna tocka na poligon
        //public IVertex GetCentroidByIdPoligon(string poligon)
        //{
        //    try
        //    {
        //        var cmd = Db.CreateCommand();
        //        if (cmd.Connection.State != ConnectionState.Open)
        //        {
        //            cmd.Connection.Open();
        //        }
        //        cmd.CommandText =

        //            "SELECT ST_AsText(ST_Centroid(ST_GeomFromText('POLYGON(( " + poligon + "))', 6316) )) ;";

               
                    
        //        //"SELECT ST_AsText(ST_Centroid(geom)) FROM mikro_opfat WHERE mikro_opfat_id=:id;";
        //        //Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);
        //        var result = Db.ExecuteScalar(cmd);
        //        result = result.Remove(0, 6); // remove POINT((
        //        result = result.Remove(result.Length - 1); // remove ))
        //        return CreateVertex(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex);
        //        throw new Exception(ex.Message);
        //    }
        //}

        //private static IVertex CreateVertex(string item)
        //{
        //    var pair = item.Split(' ');
        //    var v = new Vertex
        //    {
        //        X = Double.Parse(pair[0], new CultureInfo("en-US")),
        //        Y = Double.Parse(pair[1], new CultureInfo("en-US"))
        //    };
        //    return v;
        //}
        

    }
}