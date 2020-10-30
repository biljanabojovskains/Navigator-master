using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PublicNavigator.Dal.Abstract;
using NLog;
using System.Data;
using NpgsqlTypes;
using PublicNavigator.Models.Abstract;
using PublicNavigator.Models.Concrete;
using System.Globalization;
using PublicNavigator.Helpers;
using System.Configuration;

namespace PublicNavigator.Dal.Concrete
{
    public class NotifikaciiDa : INotifikaciiDal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public List<ITema> GetListTemi()
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
                    "select * from temi_interes;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<ITema> list = (from DataRow dr in dt.Rows select CreateTemaObject(dr)).ToList();
            return list;
        }
        private ITema CreateTemaObject(DataRow dr)
        {
            var tema = new Tema
            {
                id = int.Parse(dr["tema_id"].ToString()),
                text = dr["ime_tema"].ToString()
            };
            return tema;
        }
        public List<IPodTema> GetListPodTema(int temaId)
        {
            DataTable dt;

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select * from podtemi_interes where fk_tema=:id;";

                Db.CreateParameterFunc(cmd, "@id", temaId, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<IPodTema> list = (from DataRow dr in dt.Rows select CreatePodTemaObject(dr)).ToList();

            return list;
        }
        private static IPodTema CreatePodTemaObject(DataRow dr)
        {
            var podTema = new PodTema
            {
                id = int.Parse(dr["podtema_id"].ToString()),
                text = dr["ime_podtema"].ToString()
            };
            return podTema;
        }
        public bool Insert(int fkPodtema, int fkUser, string coordinates,string email)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }             
                cmd.CommandText =
                 "insert into tocki_od_interes(fk_podtema, fk_user, geom)  values (:fkp, :fku, ST_SetSRID(ST_MakePoint(" + coordinates +"),6316));";
                Db.CreateParameterFunc(cmd, "@fkp", fkPodtema, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@fku", fkUser, NpgsqlDbType.Integer);
                var rowsAffected = Db.ExecuteNonQuery(cmd);
                string msgBody = String.Format("Почитувани," + "<br/>" + "Успешно внесовте точка од интерес на ГИС порталот.");
                Mail.SendMail(email, ConfigurationManager.AppSettings["mailUser"],
                    "Точки од интерес", msgBody);

                
                return rowsAffected == 1;

            }

            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        public int CheckCounerPoi(int userId)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText = "SELECT COUNT(*) FROM tocki_od_interes WHERE fk_user=:id";
                Db.CreateParameterFunc(cmd, "@id", userId, NpgsqlDbType.Integer);
                var counter = int.Parse(Db.ExecuteScalar(cmd));
                return counter;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        internal List<ITockiOdInteres> GetPoiByUser(int userId)
        {
            DataTable dt;

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select p.tocki_id, ST_AsGeoJSON(p.geom) as geojson, pt.ime_podtema, i.ime_tema from tocki_od_interes p inner join podtemi_interes pt on pt.podtema_id= p.fk_podtema inner join temi_interes i on i.tema_id=pt.fk_tema where p.fk_user=:id;";

                Db.CreateParameterFunc(cmd, "@id", userId, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<ITockiOdInteres> list = (from DataRow dr in dt.Rows select CreateTockaOdInteres(dr)).ToList();

            return list;
        }

        private static ITockiOdInteres CreateTockaOdInteres(DataRow dr)
        {
            var podTema = new TockiOdInteres
            {
                Id = int.Parse(dr["tocki_id"].ToString()),
                GeoJson = dr["geojson"].ToString(),
                Tema = dr["ime_tema"].ToString(),
                Podtema = dr["ime_podtema"].ToString()
            };
            return podTema;
        }

        public bool DeletePoi(int id)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =
                 "delete from izvestuvanja_poi where fk_tocka=:id; delete from tocki_od_interes where tocki_id=:id;";
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