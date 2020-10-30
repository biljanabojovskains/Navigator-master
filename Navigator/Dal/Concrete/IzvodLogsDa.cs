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
    public class IzvodLogsDa
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public bool Insert(string username, string opfafIme, string brParcela, string path)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =
                  "INSERT INTO izvod_logs(username, opfat_ime, br_parcela, path) VALUES (:user, :ime, :br_par, :path);";
                Db.CreateParameterFunc(cmd, "@user", username, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@ime", opfafIme, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@br_par", brParcela, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@path", path, NpgsqlDbType.Text);
               
                var rowsAffected = Db.ExecuteNonQuery(cmd);
                return rowsAffected == 1;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        public bool InsertLogsUlici(string username, string opfafIme, string path)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =
                  "INSERT INTO izvod_logs_ulici(username, opfat_ime, path) VALUES (:user, :ime, :path);";
                Db.CreateParameterFunc(cmd, "@user", username, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@ime", opfafIme, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@path", path, NpgsqlDbType.Text);

                var rowsAffected = Db.ExecuteNonQuery(cmd);
                return rowsAffected == 1;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }
        public List<IIzvodLogs> GetAll()
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
                    "SELECT * FROM izvod_logs order by datum DESC;";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<IIzvodLogs> list = (from DataRow dr in dt.Rows select CreateUserObject(dr)).ToList();

            return list;
        }
        private static IIzvodLogs CreateUserObject(DataRow dr)
        {
            var izvodLog = new IzvodLogs
            {
                LogId = int.Parse(dr["log_id"].ToString()),
                UserName = dr["username"].ToString(),
                OpfatIme = dr["opfat_ime"].ToString(),
                BrParcela = dr["br_parcela"].ToString(),
                Datum = DateTime.Parse(dr["datum"].ToString()),
                Path = dr["path"].ToString()
            };
            return izvodLog;
        }
    }
}