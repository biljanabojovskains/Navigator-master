using System;
using System.Collections.Generic;
using System.Data;
using Navigator.Dal.Abstract;
using Navigator.Models.Abstract;
using Navigator.Models.Concrete;
using NLog;
using NpgsqlTypes;

namespace Navigator.Dal.Concrete
{
    public class NumerickiPokazateliDa : INumerickiPokazateliDal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public IUslov GetNumericki(int id)
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
                    "select * from numericki_pokazateli where numericki_pokazateli_id=:id;";
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

        public int AddNumeric(string path)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "INSERT INTO numericki_pokazateli(path) VALUES (:p) RETURNING numericki_pokazateli_id;";
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

        public bool AddNumerickiPokazateli(List<int> ids, int uslovId)
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
                    "UPDATE mikro_gradezni_parceli SET fk_numericki_pokazateli=:npid WHERE mikro_gradezna_parcela_id = ANY(:id_list);";
                Db.CreateParameterFunc(cmd, "@npid", uslovId, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@id_list", ids, NpgsqlDbType.Array | NpgsqlDbType.Integer);
                rowsAffected = Db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return rowsAffected > 0;
        }

        private IUslov CreateObject(DataRow dr)
        {
            var item = new Uslov
            {
                Id = int.Parse(dr["numericki_pokazateli_id"].ToString()),
                Path = dr["path"].ToString()
            };
            return item;
        }


        public IUslov Get(int id)
        {
            throw new NotImplementedException();
        }

        public int Add(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}