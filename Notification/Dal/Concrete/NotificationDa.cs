using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Notification.Dal.Abstract;
using Notification.Models.Abstract;
using Notification.Models.Concrete;
using NpgsqlTypes;

namespace Notification.Dal.Concrete
{
    class NotificationDa : INotificationDal
    {
        public List<INotifications> GetAll()
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
                    "select * from tocki_od_interes t, izvestuvanja i, users_web u, podtemi_interes p, temi_interes te where ST_Intersects(ST_Buffer(t.geom, 200), i.geom) and u.user_id=t.fk_user and i.fk_podtema=p.podtema_id and p.fk_tema=te.tema_id;";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            List<INotifications> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();

            return list;
        }

        private static INotifications CreateObject(DataRow dr)
        {
            var notification = new Notifications
            {
                TockaId = int.Parse(dr["tocki_id"].ToString()),
                IzvestuvanjeId = int.Parse(dr["izvestuvanje_id"].ToString()),
                FullName = dr["fullname"].ToString(),
                Email = dr["email"].ToString(),
                Text = dr["komentar"].ToString(),
                DateDo = DateTime.Parse(dr["datum_do"].ToString()),
                DateOd = DateTime.Parse(dr["datum_od"].ToString()),
                Tema = dr["ime_tema"].ToString(),
                Podtema = dr["ime_podtema"].ToString()
            };
            return notification;
        }

        private static ISentNotification CreateSentObject(DataRow dr)
        {
            var notification = new SentNotification
            {
                TockaId = int.Parse(dr["fk_tocka"].ToString()),
                IzvestuvanjeId = int.Parse(dr["fk_izvestuvanje"].ToString())
            };
            return notification;
        }

        public bool Update(int tockaId, int izvestuvanjeId)
        {
            int rowsAffected;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "insert into izvestuvanja_poi(fk_tocka, fk_izvestuvanje) VALUES (:tid, :iid);";

                Db.CreateParameterFunc(cmd, "@tid", tockaId, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@iid", izvestuvanjeId, NpgsqlDbType.Integer);

                rowsAffected = Db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rowsAffected == 1;
        }


        public List<ISentNotification> GetAllSent()
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
                    "select * from izvestuvanja_poi;";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            List<ISentNotification> list = (from DataRow dr in dt.Rows select CreateSentObject(dr)).ToList();

            return list;
        }
    }
}
