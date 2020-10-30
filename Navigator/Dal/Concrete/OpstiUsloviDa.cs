using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Navigator.Dal.Abstract;
using Navigator.Models.Abstract;
using Navigator.Models.Concrete;
using NLog;
using NpgsqlTypes;

namespace Navigator.Dal.Concrete
{
    public class OpstiUsloviDa : IOpstiUsloviDal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IUslov Get(int id)
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
                    "select * from opsti_uslovi where opsti_uslovi_id=:id;";
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

        public IUslov GetByOpfat(int id)
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
                    "select * from opsti_uslovi ou inner join mikro_gradezni_parceli gp ON ou.opsti_uslovi_id=gp.fk_opsti_uslovi where gp.fk_mikro_opfat=:id LIMIT 1;";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            
            if (dt==null || dt.Rows.Count == 0)
                return null;
            else
               return CreateObject(dt.Rows[0]);
        }

        private IUslov CreateObject(DataRow dr)
        {
            var item = new Uslov
            {
                Id = int.Parse(dr["opsti_uslovi_id"].ToString()),
                Path = dr["path"].ToString()
            };
            return item;
        }

        public int Add(string path)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "INSERT INTO opsti_uslovi(path) VALUES (:p) RETURNING opsti_uslovi_id;";
                Db.CreateParameterFunc(cmd, "@p", path, NpgsqlDbType.Text);

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


        public bool Add(int opfatId, int uslovId)
        {
            int rowsAffected = -1;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "UPDATE mikro_gradezni_parceli SET fk_opsti_uslovi=:ouid WHERE fk_mikro_opfat = :opfatid;";
                Db.CreateParameterFunc(cmd, "@ouid", uslovId, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@opfatid", opfatId, NpgsqlDbType.Integer);
                rowsAffected = Db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return rowsAffected > 0;
        }


        public List<string> Prep()
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
                    "select distinct(imetabela || '_' || pos_uslov) from mikro_gradezni_parceli;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<string> list = (from DataRow dr in dt.Rows select dr[0].ToString()).ToList();
            return list;
        }
    }
}
