using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Navigator.Dal.Abstract;
using Navigator.Models.Abstract;
using Navigator.Models.Concrete;
using NLog;
using NpgsqlTypes;
using Npgsql;
namespace Navigator.Dal.Concrete
{
    public class LegalizacijaDal : ILegalizacijaDal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public bool InsertDoc(string path, string filename, int legalizacija_id)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =
                 "insert into legalizacija_doc(path, date_insert, filename, fk_legalizacija_id)  select :path, :date, :filename, :legid;";

                Db.CreateParameterFunc(cmd, "@path", path, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@date", DateTime.Now, NpgsqlDbType.Date);
                Db.CreateParameterFunc(cmd, "@filename", filename, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@legid", legalizacija_id, NpgsqlDbType.Integer);

                var rowsAffected = Db.ExecuteNonQuery(cmd);
                return rowsAffected == 1;
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
           
        }

        public List<ILegalizacija> Count()
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "SELECT 'legalizirani' legalizacija, COUNT (*) FROM legalizacija_za_gradba WHERE active=FALSE union SELECT 'tekovni' legalizacija, COUNT (*) FROM legalizacija_za_gradba WHERE active=true ";


                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<ILegalizacija> list = (from DataRow dr in dt.Rows select CreateLegObject(dr)).ToList();
            return list;
        }



        public int InsertLegalizacija(string katastarskaOpstina, string katastarskaParcela, string broj, string namenaobjekt, string tipLegalizacija, string coordinates, int? brojObjekt)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =
               @"INSERT into legalizacija_za_gradba (katastarska_opstina, br_katastarska_parcela,  br_predmet, namena_na_objekt, br_objekt , tip_legalizacija, geom)
            select  :kat_opst, :kat_parc, :br_pred, :namena_objekt, :br_objekt , :tip_leg , ST_MakeValid(ST_SetSRID(ST_GeomFromGeoJSON('" + coordinates + "'), 6316)) RETURNING legalizacija_id;";
                Db.CreateParameterFunc(cmd, "@kat_opst", katastarskaOpstina, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@kat_parc", katastarskaParcela, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@br_pred", broj, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@namena_objekt", namenaobjekt, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@br_objekt", brojObjekt, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@tip_leg", tipLegalizacija, NpgsqlDbType.Text);
                //Db.CreateParameterFunc(cmd, "@docid", docId, NpgsqlDbType.Integer);

                var id = int.Parse(Db.ExecuteScalar(cmd));
                if (id > 0)
                    return id;
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);

            }
            return -1;
        }
        public bool UpdateStatusGradba(int gradbaId)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =
               @"update legalizacija_za_gradba set active=false where active=true and legalizacija_id=:id ;";
                Db.CreateParameterFunc(cmd, "@id", gradbaId, NpgsqlDbType.Integer);

                var rowsAffected = Db.ExecuteNonQuery(cmd);
                return rowsAffected == 1;
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

        }

        public List<ILegalizacija> GetLegalizacija(string coordinates)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select *, ST_AsGeoJSON(geom) as geojson from legalizacija_za_gradba  where ST_Contains(geom, ST_SetSRID(ST_MakePoint(" + coordinates + "),6316))";
                //"select *, ST_AsGeoJSON(geom) as geojson from legalizacija_za_gradba g where ST_Contains(g.geom, (select geom as geo from legalizacija_za_gradba where ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),geom))) ";


                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<ILegalizacija> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        public bool Delete(int id)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =
                 "delete from legalizacija_za_gradba where legalizacija_id=:id;";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);

                var rowsAffected = Db.ExecuteNonQuery(cmd);
                return rowsAffected == 1;
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }



        private ILegalizacija CreateObject(DataRow dr)
        {
            var legalizacija = new Legalizacija
            {
                Id = int.Parse(dr["legalizacija_id"].ToString()),
                //FkParcela = (dr["fk_parcela"] == null || dr["fk_parcela"].ToString() == "") ? (int?)null : Int32.Parse(dr["fk_parcela"].ToString()),
                KatastarskaOpstina = (dr["katastarska_opstina"].ToString()),
                BrKatastarskaParcela = dr["br_katastarska_parcela"].ToString(),
                BrPredmet = dr["br_predmet"].ToString(),
                NamenaNaObjekt = dr["namena_na_objekt"].ToString(),
                BrojObjekt = (dr["br_objekt"] == null || dr["br_objekt"].ToString() == "")
                    ? (int?)null
                    : Int32.Parse(dr["br_objekt"].ToString()),
                TipLegalizacija = dr["tip_legalizacija"].ToString(),
                GeoJson = dr["geom"].ToString(),
                Active = bool.Parse(dr["active"].ToString())
            };
            try
            {
                legalizacija.GeoJsonParceli = dr["geojson"].ToString();
                legalizacija.Path = dr["path"].ToString();
                legalizacija.Filename = dr["filename"].ToString();
            }
            catch
            {
                // ignored
            }
            return legalizacija;
        }

        private ILegalizacija CreateDocObject(DataRow dr)
        {
            var legalizacija = new Legalizacija
            {
                Id = int.Parse(dr["document_id"].ToString()),
                Path = (dr["path"].ToString()),
                Datum = DateTime.Parse(dr["date_insert"].ToString()),
                Filename = (dr["filename"].ToString()),


            };
            return legalizacija;
        }


        private ILegalizacija CreateLegObject(DataRow dr)
        {
            var legalizacija = new Legalizacija
            {
                Count = int.Parse(dr["count"].ToString()),
                Legalizirani = dr["legalizacija"].ToString(),

            };
            return legalizacija;
        }

        public List<ILegalizacija> GetKatParceliLegalizacija(string searchedText)
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
                    @"SELECT *, ST_AsGeoJSON(geom) as geojson FROM legalizacija_za_gradba WHERE br_katastarska_parcela ILIKE :searchedText";
                Db.CreateParameterFunc(cmd, "@searchedText", '%' + searchedText + '%', NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }

            List<ILegalizacija> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();

            return list;
        }
        public List<ILegalizacija> GetBrPremetLegalizacija(string searchedText)
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
                    @"SELECT *, ST_AsGeoJSON(geom) as geojson FROM legalizacija_za_gradba WHERE br_predmet ILIKE :searchedText";
                Db.CreateParameterFunc(cmd, "@searchedText", '%' + searchedText + '%', NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }

            List<ILegalizacija> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();

            return list;
        }
        public List<ITema> GetListMunicipalities()
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
                    "select * from kat_opstini;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<ITema> list = (from DataRow dr in dt.Rows select CreateOpstinaObject(dr)).ToList();
            return list;
        }
        public List<ITema> GetListNamena()
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
                    "select distinct namena_na_objekt from legalizacija_za_gradba order by namena_na_objekt asc;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<ITema> list = (from DataRow dr in dt.Rows select CreateNamenaObject(dr)).ToList();
            return list;
        }
        private ITema CreateOpstinaObject(DataRow dr)
        {
            var tema = new Tema
            {
                Id = int.Parse(dr["kat_opstina_id"].ToString()),
                ImeTema = dr["kat_opstina_ime"].ToString(),
            };
            return tema;
        }
        private ITema CreateNamenaObject(DataRow dr)
        {
            var tema = new Tema
            {
                ImeTema = dr["namena_na_objekt"].ToString()
            };
            return tema;
        }

      

             public List<ILegalizacija> ListDokumenti(int id)
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
                    @"select * from legalizacija_doc where fk_legalizacija_id=:id order by document_id desc";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }

            List<ILegalizacija> list = (from DataRow dr in dt.Rows select CreateDocObject(dr)).ToList();

            return list;
        }
             public bool DeleteDocument(int id)
             {
                 try
                 {
                     var cmd = Db.CreateCommand();
                     if (cmd.Connection.State != ConnectionState.Open)
                     {
                         cmd.Connection.Open();
                     }

                     cmd.CommandText =
                      "delete from legalizacija_doc where document_id=:id;";
                     Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);

                     var rowsAffected = Db.ExecuteNonQuery(cmd);
                     return rowsAffected == 1;
                 }

                 catch (Exception ex)
                 {
                     Logger.Error(ex);
                     throw new Exception(ex.Message);
                 }
             }
    }

}