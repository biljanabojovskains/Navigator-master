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
    public class TehnickiIspravkiDa : ITehnickiIspravkiDal
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
                    "select * from tehnicki_ispravki where tehnicki_ispravki_id=:id;";
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

        private IUslov CreateObject(DataRow dr)
        {
            var item = new Uslov
            {
                Id = int.Parse(dr["tehnicki_ispravki_id"].ToString()),
                Path = dr["path"].ToString()
            };
            return item;
        }
      

        public int Add(string filePath)
        {
            throw new NotImplementedException();
        }

    }
}