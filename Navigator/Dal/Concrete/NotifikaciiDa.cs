using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Navigator.Dal.Abstract;
using NLog;
using System.Data;
using NpgsqlTypes;
using Navigator.Models.Abstract;
using Navigator.Models.Concrete;
using System.Globalization;


namespace Navigator.Dal.Concrete
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
                Id = int.Parse(dr["tema_id"].ToString()),
                ImeTema = dr["ime_tema"].ToString(),
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
                text = dr["ime_podtema"].ToString(),
            };
            return podTema;
        }
        public bool Insert(int fkPodtema, int fkUser, string komentar, DateTime datumOd, DateTime datumDo, string coordinates)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }             
                cmd.CommandText =
                 "insert into izvestuvanja(fk_podtema, fk_user, komentar, datum_od, datum_do, geom)  select :fkp, :fku, :kom, :od, :do, ST_SetSRID(ST_GeomFromGeoJSON('" + coordinates + "'),6316);";
                Db.CreateParameterFunc(cmd, "@fkp", fkPodtema, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@fku", fkUser, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@kom", komentar, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@od", datumOd.Date, NpgsqlDbType.Date);
                Db.CreateParameterFunc(cmd, "@do", datumDo.Date, NpgsqlDbType.Date);
                

                var rowsAffected = Db.ExecuteNonQuery(cmd);
                return rowsAffected == 1;
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }
        public List<INotifikacii> GetAllNotifications()
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
                    "select *, ST_AsGeoJSON(geom) as geojson from izvestuvanja AS izv inner join podtemi_interes as pod on izv.fk_podtema = pod.podtema_id inner join temi_interes as tem on pod.fk_tema = tem.tema_id;";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<INotifikacii> list = (from DataRow dr in dt.Rows select CreateNotificationObject(dr)).ToList();

            return list;
        }
        private static INotifikacii CreateNotificationObject(DataRow dr)
        {
            var notifikacii = new Notifikacii
            {            
                Id = int.Parse(dr["izvestuvanje_id"].ToString()),
                Tema = dr["ime_tema"].ToString(),
                Podtema = dr["ime_podtema"].ToString(),
                DatumOd = DateTime.Parse(dr["datum_od"].ToString()),
                DatumDo = DateTime.Parse(dr["datum_do"].ToString()),
                GeoJson = dr["geojson"].ToString()
            };
            return notifikacii;
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
                 "delete from izvestuvanja where izvestuvanje_id=:id;";
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